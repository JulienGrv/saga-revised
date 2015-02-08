using Saga.Enumarations;
using Saga.Map;
using Saga.PrimaryTypes;
using Saga.Shared.Definitions;
using Saga.Structures;
using Saga.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace Saga.Scripting
{
    /// <summary>
    /// AI Class for the ship. Ships are always visible so with the RegionVisibility
    /// attribute we declare them to be be always visible. The RegionVisibility is
    /// inheritable and by default is declared on per region.
    ///
    /// On subscribing and unsubscribing the object on the regiontree the RegionVisibility
    /// is checked as a result with the easyness of just 1 call to Regiontree.Subscribe
    /// you can subscribe your object to become visible instead of 1 for always visible and
    /// 1 for region-based visible.
    /// </summary>
    [RegionVisibility(Level = VisibilityLevel.Always)]
    internal class Ship : BaseMob, IArtificialIntelligence
    {
        #region Private Members

        private Path aipath;

        #endregion Private Members

        #region Base Members

        public override void OnRegister()
        {
            this.currentzone.Regiontree.Subscribe(this);
            string filename = Server.SecurePath("~/ships/{0}.xml", this.ModelId);
            aipath = Path.FromFile(filename, this);
            if (aipath != null)
            {
                aipath.Start(this);
                LifespanAI.Subscribe(this);
            }
        }

        public override void OnDeregister()
        {
            if (LifespanAI.IsSubscribed(this))
            {
                LifespanAI.Unsubscribe(this);
            }

            base.OnDeregister();
        }

        public override void Appears(Character character)
        {
            aipath.Show(this, character);
        }

        #endregion Base Members

        #region IArtificialIntelligence

        void IArtificialIntelligence.Process()
        {
            //DONOTHING
            aipath.Check(this);
        }

        bool IArtificialIntelligence.IsActivatedOnDemand
        {
            get { return false; }
        }

        private Saga.Tasks.LifespanAI.Lifespan lifespan = new Saga.Tasks.LifespanAI.Lifespan();

        Saga.Tasks.LifespanAI.Lifespan IArtificialIntelligence.Lifespan
        {
            get { return lifespan; }
        }

        #endregion IArtificialIntelligence
    }

    /// <summary>
    /// OOP Wrapper helper for the Ship AI
    /// </summary>
    internal class Path
    {
        #region Private Members

        private List<PathElement> Elements = new List<PathElement>();
        private int LastTick = Environment.TickCount;
        private int point;
        private int resetpoint;
        private byte state;

        #endregion Private Members

        #region Public Members

        protected void NextPoint(Ship ship)
        {
            int count = Elements.Count;
            if (count > 0)
                point = (++point % count);
            Elements[point].Start(ship);
        }

        public void Check(Ship ship)
        {
            Elements[point].Check(ship);
        }

        public void Start(Ship ship)
        {
            this.point = resetpoint;
            Elements[resetpoint].Start(ship);
        }

        public void Reset(Ship ship)
        {
            this.point = resetpoint;
            Elements[resetpoint].Start(ship);
        }

        public void Show(Ship ship, Character character)
        {
            Elements[point].Show(ship, character);
        }

        #endregion Public Members

        #region Public Static

        public static Path FromFile(string filename, Ship ship)
        {
            ushort speed = (ushort)ship.Status.WalkingSpeed;
            float lastx = ship.Position.x;
            float lasty = ship.Position.y;
            float lastz = ship.Position.z;
            Rotator lastyaw = ship.Yaw;
            Path path = new Path();
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (XmlTextReader reader = new XmlTextReader(fs))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name.ToUpperInvariant())
                            {
                                case "POINT":
                                    lastx = float.Parse(reader["x"], CultureInfo.InvariantCulture);
                                    lasty = float.Parse(reader["y"], CultureInfo.InvariantCulture);
                                    lastz = float.Parse(reader["z"], CultureInfo.InvariantCulture);
                                    lastyaw = int.Parse(reader["yaw"], CultureInfo.InvariantCulture);
                                    PointElement p = new PointElement(path,
                                        lastx, lasty, lastz, lastyaw);
                                    break;

                                case "RESETPOSITION":
                                    lastx = float.Parse(reader["x"], CultureInfo.InvariantCulture);
                                    lasty = float.Parse(reader["y"], CultureInfo.InvariantCulture);
                                    lastz = float.Parse(reader["z"], CultureInfo.InvariantCulture);
                                    lastyaw = int.Parse(reader["yaw"], CultureInfo.InvariantCulture);
                                    ResetElement r = new ResetElement(path,
                                        lastx, lasty, lastz, lastyaw);
                                    break;

                                case "SET":
                                    speed = ushort.Parse(reader["speed"], CultureInfo.InvariantCulture);
                                    SetElement s = new SetElement(path, speed);
                                    break;

                                case "SHIPYARD":
                                    ShipYard y = new ShipYard(path,
                                        new Point(lastx, lasty, lastz), lastyaw,
                                        new Point(
                                        float.Parse(reader["x"], CultureInfo.InvariantCulture),
                                        float.Parse(reader["y"], CultureInfo.InvariantCulture),
                                        float.Parse(reader["z"], CultureInfo.InvariantCulture)),
                                        byte.Parse(reader["to"], CultureInfo.InvariantCulture),
                                        uint.Parse(reader["shiptime"], CultureInfo.InvariantCulture),
                                        uint.Parse(reader["docktime"], CultureInfo.InvariantCulture));
                                    break;

                                case "START":
                                    ship.Position = new Point(lastx, lasty, lastz);
                                    ship.Yaw = ship.Yaw;
                                    path.resetpoint = path.Elements.Count;
                                    ship.Status.WalkingSpeed = speed;
                                    break;
                            }
                            break;
                    }
                }
            }

            return path;
        }

        #endregion Public Static

        #region Nested

        protected abstract class PathElement
        {
            private Path owner;

            public Path Owner
            {
                get
                {
                    return owner;
                }
            }

            public abstract void Start(Ship ship);

            public abstract void Check(Ship ship);

            public abstract void Show(Ship ship, Character character);

            public PathElement(Path owner)
            {
                this.owner = owner;
                owner.Elements.Add(this);
            }
        }

        protected class PointElement : PathElement
        {
            private WaypointStructure structure;

            public override void Check(Ship ship)
            {
                int tdiff = Environment.TickCount - Owner.LastTick;
                if (Owner.state == 0)
                {
                    if (tdiff > 1000)
                    {
                        UpdateMovment(tdiff, ship);
                        int speed = (int)ship.Status.WalkingSpeed;
                        double distance = ComputeDistance(ship);
                        if (distance < speed)
                        {
                            int t_add = (int)((distance / (double)speed) * 1000);
                            ship.Yaw = this.structure.rotation;
                            ship.Position = this.structure.point;
                            Owner.LastTick = Environment.TickCount + t_add;
                            Owner.NextPoint(ship);
                            Owner.state = 1;
                        }
                        else
                        {
                            Owner.LastTick = Environment.TickCount;
                        }
                    }
                }
                else
                {
                    if (tdiff > 0)
                    {
                        UpdateMovment(tdiff, ship);
                        Regiontree.UpdateRegion(ship, false);
                        int numberofpoints = 0;
                        for (int i = Owner.point; i < Owner.Elements.Count; i++)
                        {
                            if (Owner.Elements[i] is PointElement)
                                numberofpoints++;
                            else
                                break;
                        }

                        Owner.state = 0;
                        if (numberofpoints > 1)
                        {
                            WaypointStructure current = new WaypointStructure(ship.Position, ship.Yaw);
                            PointElement A = Owner.Elements[Owner.point + 0] as PointElement;
                            PointElement B = Owner.Elements[Owner.point + 1] as PointElement;
                            ship.WideMovement(current, A.structure, B.structure);
                        }
                    }
                }
            }

            public override void Start(Ship ship)
            {
            }

            public double ComputeDistance(Ship ship)
            {
                Point A = ship.Position;
                Point B = structure.point;
                double dx = (double)(A.x - B.x);
                double dy = (double)(A.y - B.y);
                double dz = (double)(A.z - B.z);
                double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                return distance;
            }

            public void UpdateMovment(int tdiff, Ship ship)
            {
                ushort yaw = Point.CalculateYaw(ship.Position, this.structure.point);
                double diff = tdiff * ((double)ship.Status.WalkingSpeed / (double)1000);
                Point Loc = ship.Position;
                Loc.x += (float)(diff * Math.Cos(yaw * (Math.PI / 32768)));
                Loc.y += (float)(diff * Math.Sin(yaw * (Math.PI / 32768)));
                Loc.z = ship.Position.z;
                ship.Position = Loc;
            }

            public Point NextMovement(int tdiff, Ship ship)
            {
                ushort yaw = Point.CalculateYaw(ship.Position, this.structure.point);
                float diff = tdiff * ((float)ship.Status.WalkingSpeed / (float)1000);
                Point Loc = ship.Position;
                Loc.x += (float)(diff * Math.Cos(yaw * (Math.PI / 32768)));
                Loc.y += (float)(diff * Math.Sin(yaw * (Math.PI / 32768)));
                Loc.z = ship.Position.z;
                return Loc;
            }

            public PointElement(Path owner, float x, float y, float z, Rotator yaw)
                : base(owner)
            {
                structure.point = new Point(x, y, z);
                structure.rotation = yaw;
            }

            public override void Show(Ship ship, Character character)
            {
                int numberofpoints = 0;
                for (int i = Owner.point; i < Owner.Elements.Count; i++)
                {
                    if (Owner.Elements[i] is PointElement)
                        numberofpoints++;
                    else
                        break;
                }

                int t_diff = Environment.TickCount - Owner.LastTick;
                if (numberofpoints > 1 && t_diff > -1)
                {
                    WaypointStructure current = new WaypointStructure(ship.Position, ship.Yaw);
                    PointElement A = Owner.Elements[Owner.point + 0] as PointElement;
                    PointElement B = Owner.Elements[Owner.point + 1] as PointElement;
                    ship.WideMovement(character, current, A.structure, B.structure);
                }
                else if (numberofpoints > 0 && t_diff > -1)
                {
                    WaypointStructure current = new WaypointStructure(ship.Position, ship.Yaw);
                    PointElement A = Owner.Elements[Owner.point + 0] as PointElement;
                    ship.WideMovement(character, current, A.structure);
                }
            }
        }

        protected class SetElement : PathElement
        {
            private ushort speed;

            public override void Check(Ship ship)
            {
                //Do nothing
            }

            public override void Start(Ship ship)
            {
                ship.Status.WalkingSpeed = speed;
                Owner.NextPoint(ship);
            }

            public SetElement(Path owner, ushort speed)
                : base(owner)
            {
                this.speed = speed;
            }

            public override void Show(Ship ship, Character character)
            {
                //throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Queries pending characters and warps new pending characters
        /// </summary>
        protected class ShipYard : PathElement
        {
            private uint docktime = 60000;
            private int TravelTime = 18000;
            public byte DestinationMapId;
            public Point DepartureWarpPosition;
            public BoundingBox box;

            public override void Check(Ship ship)
            {
                int t_diff = Environment.TickCount - Owner.LastTick;
                if (t_diff > 60000)
                {
                    //Check for players to warp
                    List<Character> characters = new List<Character>();
                    Regiontree tree = ship.currentzone.Regiontree;
                    foreach (Character regionObject in tree.SearchActors(ship, SearchFlags.Characters))
                    {
                        if (box.IsInBox(regionObject.Position))
                        {
                            characters.Add(regionObject);
                        }
                    }

                    if (characters.Count > 0)
                        Saga.Tasks.Shipservice.Enqeuee
                        (
                            characters,
                            ship.currentzone.Map,
                            DepartureWarpPosition,
                            DestinationMapId,
                            TravelTime
                         );

                    Thread.Sleep(5);

                    Owner.NextPoint(ship);
                    Owner.LastTick = Environment.TickCount;
                }
            }

            public override void Start(Ship ship)
            {
                byte zone = ship.currentzone.Map;
                Saga.Tasks.Shipservice.Deqeuee(DepartureWarpPosition, zone, DestinationMapId);
            }

            public ShipYard(Path owner, Point a, Rotator yaw, Point destination, byte destinationmap, uint Traveltime, uint docktime)
                : base(owner)
            {
                this.box = new BoundingBox
                (
                    4000,   //Width of the boat
                    2000,   //Length of the boat
                    410,    //Height of the boat
                    a - new Point(0, 0, 300),
                    yaw.rotation    //Yaw of the boat
                );

                DepartureWarpPosition = destination;
                DestinationMapId = destinationmap;
                this.docktime = docktime;
            }

            public override void Show(Ship ship, Character character)
            {
                //throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Resets the position to a absolute position
        /// </summary>
        protected class ResetElement : PathElement
        {
            private Point point;
            private Rotator yaw;

            public override void Check(Ship ship)
            {
                //Do nothing
            }

            public override void Start(Ship ship)
            {
                ship.Position = point;
                ship.Yaw = yaw;
                Regiontree.UpdateRegion(ship, false);
                Owner.NextPoint(ship);
            }

            public ResetElement(Path owner, float x, float y, float z, Rotator yaw)
                : base(owner)
            {
                this.yaw = yaw;
                this.point = new Point(x, y, z);
            }

            public override void Show(Ship ship, Character character)
            {
                //throw new NotImplementedException();
            }
        }

        #endregion Nested
    }
}