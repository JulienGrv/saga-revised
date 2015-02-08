using Saga.Configuration;
using Saga.Enumarations;
using Saga.Map.Configuration;
using Saga.PrimaryTypes;
using Saga.Structures;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;

namespace Saga.Factory
{
    /// <summary>
    /// Factory that interacts with applying and deapplying additions.
    /// </summary>
    /// <remarks>
    /// This factory compiles the additions to ILCode which can be recollected
    /// to the garbage collector in time when reloading.
    /// </remarks>
    public class Additions : FactoryBase
    {
        protected static BooleanSwitch refenceWarningsAsErrors = new BooleanSwitch("AdditionReferenceWarningsAsErrors", "Treat warnings as errors in the refence files", "0");

        #region Ctor/Dtor

        public Additions()
        {
        }

        ~Additions()
        {
            this._additions = null;
            this.addition_table = null;
        }

        #endregion Ctor/Dtor

        #region Internal Members

        /// <summary>
        /// Dictonairy to contain all additions
        /// </summary>
        protected Dictionary<uint, Info> _additions;

        /// <summary>
        /// Dictonairy to contain all delegates (this is cleared afterwards)
        /// </summary>
        protected Dictionary<uint, MethodInfo> addition_table;

        #endregion Internal Members

        #region Private Methods

        /// <summary>
        /// Actually applies the addition
        /// </summary>
        /// <param name="addition">Addition refetence</param>
        /// <param name="target">Target who to apply to</param>
        /// <param name="apply">Boolean indicating to apply or deapply</param>
        private void DoAddition(ref Info addition, Actor target, AdditionContext apply)
        {
            addition.Do(target, target, apply);
        }

        #endregion Private Methods

        #region Protected Methods

        protected override void Initialize()
        {
            _additions = new Dictionary<uint, Info>();
            addition_table = new Dictionary<uint, MethodInfo>();
        }

        protected override void Load()
        {
            AdditionSettings section = (AdditionSettings)ConfigurationManager.GetSection("Saga.Factory.Addition");
            if (section != null)
            {
                try
                {
                    string file = Saga.Structures.Server.SecurePath(section.Reference);
                    if (File.Exists(file))
                        ParseReferenceAsCsvStream(File.OpenRead(file));
                }
                catch (IOException e)
                {
                    HostContext.AddUnhandeldException(e);
                }
                catch (Exception e)
                {
                    HostContext.AddUnhandeldException(e);
                }

                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("AdditionFactory", "Loading addition information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
            else
            {
                WriteWarning("XmlSections", "Section Saga.Factory.Addition was not found");
            }
        }

        protected override void FinishedLoading()
        {
            foreach (KeyValuePair<uint, MethodInfo> pair in this.addition_table)
                GC.ReRegisterForFinalize(pair.Value);
            addition_table.Clear();
            base.FinishedLoading();
        }

        protected virtual void ParseReferenceAsCsvStream(System.IO.Stream stream)
        {
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    CommaDelimitedString fields = CommaDelimitedString.Parse(c.ReadLine());
                    uint AdditionId = 0;
                    try
                    {
                        string field = fields[1].Trim('\0', ' ');
                        bool IsValidInteger = uint.TryParse(fields[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out AdditionId);
                        AdditionHandler Handler = CoreService.Find<AdditionHandler>(field);

                        if (!IsValidInteger)
                        {
                            if (refenceWarningsAsErrors.Enabled)
                                WriteError("AdditionFactory", "Not a valid integer {0}", field);
                            else
                                WriteWarning("AdditionFactory", "Not a valid integer {0}", field);
                        }
                        else if (Handler == null || Handler.Method == null)
                        {
                            if (refenceWarningsAsErrors.Enabled)
                                WriteError("AdditionFactory", "Method not found {0}", field);
                            else
                                WriteWarning("AdditionFactory", "Method not found {0}", field);
                        }
                        else
                        {
                            MethodInfo info = Handler.Method;
                            GC.SuppressFinalize(info);
                            this.addition_table.Add(AdditionId, info);
                        }
                    }
                    catch (Exception e)
                    {
                        HostContext.AddUnhandeldException(e);
                    }
                }
            }
        }

        protected override void ParseAsCsvStream(System.IO.Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    ProgressReport.Invoke();
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');
                    Type additiontype = typeof(AdditionsBonus);

                    uint AdditionId = uint.Parse(fields[0], NumberFormatInfo.InvariantInfo);
                    uint[] Functions = new uint[] { (uint)Enum.Parse( additiontype, fields[1] ), (uint)Enum.Parse( additiontype, fields[2] ), (uint)Enum.Parse( additiontype, fields[3] ),
                                                    (uint)Enum.Parse( additiontype, fields[4] ), (uint)Enum.Parse( additiontype, fields[5] ), (uint)Enum.Parse( additiontype, fields[6] ),
                                                    (uint)Enum.Parse( additiontype, fields[7] ), (uint)Enum.Parse( additiontype, fields[8] ), (uint)Enum.Parse( additiontype, fields[9] ),
                                                    (uint)Enum.Parse( additiontype, fields[10] )};

                    int[] Values = new int[] {  int.Parse( fields[11],  NumberFormatInfo.InvariantInfo ),
                                                    int.Parse( fields[12], NumberFormatInfo.InvariantInfo ),
                                                    int.Parse( fields[13], NumberFormatInfo.InvariantInfo ),
                                                    int.Parse( fields[14],  NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[15],  NumberFormatInfo.InvariantInfo ),
                                                    int.Parse( fields[16], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[17],  NumberFormatInfo.InvariantInfo ),
                                                    int.Parse( fields[18], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[19], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[20], NumberFormatInfo.InvariantInfo)};

                    Info info = Info.From(addition_table, Functions, Values);
                    if (info != null)
                    {
                        _additions.Add(AdditionId, info);
                    }
                }
            }
        }

        protected override void ParseAsXmlStream(Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                Info info = null;
                uint AdditionId = 0;
                uint Interval = 0;
                uint EffectDuration = 0;
                string value = null;
                uint[] Functions = null;
                int[] Values = null;
                String[] fields = null;
                Type additiontype = typeof(AdditionsBonus);

                while (reader.Read())
                {
                    ProgressReport.Invoke();
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            value = null;
                            if (reader.Name.ToUpperInvariant() == "ROW")
                            {
                                Functions = null;
                                Values = null;
                                fields = null;
                                AdditionId = 0;
                                Interval = 0;
                                EffectDuration = 0;
                            }
                            break;

                        case XmlNodeType.Text:
                            value = reader.Value;
                            break;

                        case XmlNodeType.EndElement:
                            switch (reader.Name.ToUpperInvariant())
                            {
                                case "ROW": goto Add;
                                case "ID":
                                    AdditionId = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "TYPE": break;
                                case "DISPOSITION": break;
                                case "INTERVAL":
                                    Interval = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "EFFECTDURATION":
                                    EffectDuration = uint.Parse(value, NumberFormatInfo.InvariantInfo); break;
                                case "FUNCTION":
                                    fields = value.Split(',');
                                    Functions = new uint[] { (uint)Enum.Parse( additiontype, fields[0] ),
                                                    (uint)Enum.Parse( additiontype, fields[1] ),
                                                    (uint)Enum.Parse( additiontype, fields[2] ),
                                                    (uint)Enum.Parse( additiontype, fields[3] ),
                                                    (uint)Enum.Parse( additiontype, fields[4] ),
                                                    (uint)Enum.Parse( additiontype, fields[5] ),
                                                    (uint)Enum.Parse( additiontype, fields[6] ),
                                                    (uint)Enum.Parse( additiontype, fields[7] ),
                                                    (uint)Enum.Parse( additiontype, fields[8] ),
                                                    (uint)Enum.Parse( additiontype, fields[9])};
                                    break;

                                case "VALUE":
                                    fields = value.Split(',');
                                    Values = new int[] {  int.Parse( fields[0],  NumberFormatInfo.InvariantInfo ),
                                                    int.Parse( fields[1], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[2], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[3], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[4], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[5], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[6], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[7], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[8], NumberFormatInfo.InvariantInfo),
                                                    int.Parse( fields[9], NumberFormatInfo.InvariantInfo)};
                                    break;
                            }
                            break;
                    }
                    continue;
                Add:
                    try
                    {
                        info = Info.From(addition_table, Functions, Values);
                        if (info != null)
                        {
                            info.Addition = AdditionId;
                            info.Interval = Interval;
                            info.EffectDuration = EffectDuration;
                            _additions.Add(AdditionId, info);
                        }
                    }
                    catch (ArgumentException)
                    {
                        WriteError("AdditionFactory", "Duplicate id detected: {0}", AdditionId);
                        return;
                    }

                    continue;
                }
            }
        }

        protected virtual void Initialize_AdditionTable()
        {
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Checks if the specified addition is available
        /// </summary>
        /// <param name="id">Identifier of the addition</param>
        /// <returns>true if the addition is available</returns>
        public bool HasAddition(uint id)
        {
            return this._additions.ContainsKey(id);
        }

        /// <summary>
        /// Safe getting a addition
        /// </summary>
        /// <param name="id">AdditionId</param>
        /// <param name="addition">Out reference of the addition</param>
        /// <returns>True if addition was found</returns>
        public bool TryGetAddition(uint id, out Info addition)
        {
            return this._additions.TryGetValue(id, out addition);
        }

        /// <summary>
        /// Applies the addition to the specified actor
        /// </summary>
        /// <param name="id">Id of the addition</param>
        /// <param name="target">Target to who to apply</param>
        public void ApplyAddition(uint id, Actor target)
        {
            Info AdditionEffects;
            if (TryGetAddition(id, out AdditionEffects) == true)
            {
                DoAddition(ref AdditionEffects, target, AdditionContext.Applied);
            }
        }

        /// <summary>
        /// Deapplies the addition to the specified actor
        /// </summary>
        /// <param name="id">Id of the addition</param>
        /// <param name="target">Target to who to apply</param>
        public void DeapplyAddition(uint id, Actor target)
        {
            Info AdditionEffects;
            if (TryGetAddition(id, out AdditionEffects) == true)
            {
                DoAddition(ref AdditionEffects, target, AdditionContext.Deapplied);
            }
        }

        #endregion Public Methods

        #region Protected Properties

        /// <summary>
        /// Get the notification string.
        /// </summary>
        /// <remarks>
        /// We used notification strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string Notification
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_ADDITION; }
        }

        /// <summary>
        /// Get the readystate string.
        /// </summary>
        /// <remarks>
        /// We used readystate strings from the resource files. This way it's easier
        /// for us to make a multilanguagable server. And a golden rule in C# is that
        /// strings are slow, so rather have it instanted once by the resource file than
        /// reallocting a new string for every progress report.
        /// </remarks>
        protected override string ReadyState
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_ADDITION; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        /// <summary>
        /// Info class used to interact with the addition.
        /// </summary>
        public class Info
        {
            #region Private Members

            /// <summary>
            /// Method with compiled apply/deapply functions
            /// </summary>
            private ByRefDelegate del;

            /// <summary>
            /// Addition id
            /// </summary>
            private uint addition = 0;

            /// <summary>
            /// Interval between addition applies
            /// </summary>
            private uint interval = 0;

            /// <summary>
            /// Durée de l'effet
            /// </summary>
            private uint effectDuration = 0;

            #endregion Private Members

            #region Constructor / Deconstructor

            /// <summary>
            /// Constructor uses a dynamic method created by our info parser.
            /// </summary>
            /// <param name="method">Dyanmic method that applies our additions</param>
            private Info(DynamicMethod method)
            {
                this.del = (ByRefDelegate)method.CreateDelegate(typeof(ByRefDelegate));
            }

            #endregion Constructor / Deconstructor

            #region Internal Methods

            /// <summary>
            /// Compiles the addition from data
            /// </summary>
            /// <param name="handlers">Lookup table with functions</param>
            /// <param name="functions">Table of functions</param>
            /// <param name="values">Values associated with the functions</param>
            /// <returns>Addition info</returns>
            internal static Info From(Dictionary<uint, MethodInfo> handlers, uint[] functions, int[] values)
            {
                try
                {
                    MethodInfo methodInfo;
                    //DynamicMethod method = new DynamicMethod(string.Empty, typeof(void), new Type[] { typeof(AdditionValue).MakeByRefType() });
                    DynamicMethod method = new DynamicMethod(string.Empty, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), new Type[] { typeof(AdditionValue).MakeByRefType() }, typeof(Additions).Module, false);

                    method.DefineParameter(1, ParameterAttributes.Out, "test");
                    ILGenerator ilgen = method.GetILGenerator();

                    for (int i = 0; i < functions.Length; i++)
                    {
                        if (functions[i] > 0)
                        {
                            if (handlers.TryGetValue(functions[i], out methodInfo) && methodInfo != null)
                            {
                                //Parse additionValue by reference
                                ilgen.Emit(OpCodes.Ldarg_0, 0);
                                //Hardcode integer
                                ilgen.Emit(OpCodes.Ldc_I4, values[i]);
                                //Call method
                                ilgen.Emit(OpCodes.Call, methodInfo);
                            }
                        }
                    }

                    //No values on the stack and return
                    ilgen.Emit(OpCodes.Ret);
                    return new Info(method);
                }
                catch (Exception e)
                {
                    HostContext.Current.AddUnhandeldException(e);
                    return null;
                }
            }

            /// <summary>
            /// Applies the addition
            /// </summary>
            /// <param name="sender">Sender who calls the addition</param>
            /// <param name="target">Target of the sender</param>
            /// <param name="apply">Boolean indication to apply or deapply the addition</param>
            internal void Do(object sender, object target, AdditionContext apply)
            {
                try
                {
                    AdditionValue state = new AdditionValue();
                    state.context = apply;
                    state.sender = sender;
                    state.target = target;
                    state.additionid = this.Addition;
                    del(ref state);
                }
                catch (Exception e)
                {
                    Trace.TraceWarning("Critical error in addition: {0}", e.Message);
                }
            }

            #endregion Internal Methods

            #region Public Properties

            /// <summary>
            /// Gets the additionid of the addition
            /// </summary>
            public uint Addition
            {
                get
                {
                    return addition;
                }
                internal set
                {
                    addition = value;
                }
            }

            /// <summary>
            /// Gets the interval time between applying the additions
            /// </summary>
            public uint Interval
            {
                get
                {
                    return interval;
                }
                internal set
                {
                    interval = value;
                }
            }

            /// <summary>
            /// Donne la durée de l'effet
            /// </summary>
            public uint EffectDuration
            {
                get
                {
                    return effectDuration;
                }
                internal set
                {
                    effectDuration = value;
                }
            }

            #endregion Public Properties
        }

        /// <summary>
        /// Exception thrown when a skill is not found
        /// </summary>
        [Serializable()]
        public class AdditionNotFoundException : Exception { }

        /// <summary>
        /// Delegate to match addition subfunctions
        /// </summary>
        /// <param name="sender">Sender which calls the addition</param>
        /// <param name="target">Target of the sender</param>
        /// <param name="value">Value indicating the addition</param>
        /// <param name="apply">Bool saying to apply or deapply the addition</param>
        public delegate void AdditionHandler(ref AdditionValue state, int value);

        public delegate void ByRefDelegate(ref AdditionValue a);

        #endregion Nested Classes/Structures
    }
}