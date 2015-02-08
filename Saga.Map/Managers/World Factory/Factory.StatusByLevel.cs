using Saga.Configuration;
using Saga.Map.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Saga.Factory
{
    public class StatusByLevel : FactoryBase
    {
        #region Ctor/Dtor

        public StatusByLevel()
        {
        }

        #endregion Ctor/Dtor

        #region Internal Members

        public float Modifier_Cexp = 1;
        public float Modifier_Jexp = 1;
        public float Modifier_Wexp = 1;
        public float Modifier_Drate = 1;

        private uint _MaxCLVL = 99;
        private uint _MaxJLVL = 99;
        private uint _MaxWLVL = 99;
        public Dictionary<byte, Info> levelTable;

        #endregion Internal Members

        #region Protected Methods

        protected override void Initialize()
        {
            levelTable = new Dictionary<byte, Info>();
        }

        protected override void Load()
        {
            StatusByLevelSettings section = (StatusByLevelSettings)ConfigurationManager.GetSection("Saga.Factory.StatusByLevel");
            if (section != null)
            {
                Modifier_Cexp = (float)section.Cexp;
                Modifier_Jexp = (float)section.Jexp;
                Modifier_Wexp = (float)section.Wexp;
                Modifier_Drate = (float)section.Droprate;

                foreach (FactoryFileElement element in section.FolderItems)
                {
                    WriteLine("StatusByLevelFactory", "Loading statusbylevel information from: {0} using format {1}", element.Path, element.Reader);
                    LoadParameterizedStreamContent(Saga.Structures.Server.SecurePath(element.Path), element.Reader);
                }
            }
            else
            {
                WriteWarning("XmlSections", "Section {0} was not found", "Saga.Factory.StatusByLevel");
            }
        }

        protected override void ParseAsCsvStream(Stream stream, FactoryBase.ReportProgress ProgressReport)
        {
            using (StreamReader c = new StreamReader(stream))
            {
                c.ReadLine();
                while (c.Peek() > 0)
                {
                    ProgressReport.Invoke();
                    String row = c.ReadLine();
                    String[] fields = row.Split(',');
                    levelTable.Add(byte.Parse(fields[0], NumberFormatInfo.InvariantInfo), new Info(
                        uint.Parse(fields[1], NumberFormatInfo.InvariantInfo),
                        uint.Parse(fields[2], NumberFormatInfo.InvariantInfo),
                        uint.Parse(fields[3], NumberFormatInfo.InvariantInfo),
                        uint.Parse(fields[4], NumberFormatInfo.InvariantInfo),
                        uint.Parse(fields[5], NumberFormatInfo.InvariantInfo)
                        ));
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public uint FindRequiredCexp(byte level)
        {
            return this.levelTable[level].cexp;
        }

        public uint FindRequiredWexp(int level)
        {
            return this.levelTable[(byte)level].wexp;
        }

        public uint FindRequiredJexp(byte level)
        {
            return this.levelTable[level].jexp;
        }

        public uint FindRepairCosts(byte level)
        {
            try
            {
                return this.levelTable[level].RepairCosts;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public uint FindUpgradeCosts(byte level)
        {
            try
            {
                return this.levelTable[level].UpgradeCosts;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public byte FindClvlByCexp(uint cexp)
        {
            int lvl = 0;
            foreach (KeyValuePair<byte, Info> pair in this.levelTable)
            {
                if (pair.Value.cexp >= cexp) break;
                lvl = pair.Key;
            }
            return (byte)lvl;
        }

        public byte FindJlvlByJexp(uint jexp)
        {
            int lvl = 0;
            foreach (KeyValuePair<byte, Info> pair in this.levelTable)
            {
                if (pair.Value.jexp > jexp) break;
                lvl = pair.Key;
            }
            return (byte)lvl;
        }

        public byte FindClvlDifference(byte clvl, uint cexp)
        {
            int a = 0;
            for (byte i = (byte)(clvl + 1); i <= _MaxCLVL; i++)
            {
                if (cexp < this.levelTable[i].cexp) break;
                a++;
            }
            return (byte)a;
        }

        public byte FindJlvlDifference(byte jlvl, uint jexp)
        {
            int a = 0;
            for (byte i = (byte)(jlvl + 1); i <= _MaxJLVL; i++)
            {
                if (jexp < this.levelTable[i].jexp) break;
                a++;
            }
            return (byte)a;
        }

        #endregion Public Methods

        #region Public Properties

        public uint MaxCLVL { get { return this._MaxCLVL; } }

        public uint MaxJLVL { get { return this._MaxJLVL; } }

        public uint MaxWLVL { get { return this._MaxWLVL; } }

        #endregion Public Properties

        #region Protected Properties

        protected override string Notification
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_NOTIFYCATION_EXPTABLE; }
        }

        protected override string ReadyState
        {
            get { return Saga.Map.Utils.Resources.SingletonNotificationStrings.FACTORY_READYSTATE_EXPTABLE; }
        }

        #endregion Protected Properties

        #region Nested Classes/Structures

        public class Info
        {
            public uint cexp;
            public uint jexp;
            public uint wexp;
            public uint UpgradeCosts;
            public uint RepairCosts;

            public Info(uint cexp, uint jexp, uint wexp, uint upgradecosts, uint repaircosts)
            {
                this.cexp = cexp;
                this.jexp = jexp;
                this.wexp = wexp;
                this.UpgradeCosts = upgradecosts;
                this.RepairCosts = repaircosts;
            }
        }

        #endregion Nested Classes/Structures
    }
}