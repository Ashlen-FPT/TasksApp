using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TasksApp.Data;
using TasksApp.Enums;
using TasksApp.Services;
using TasksApp.ViewModels;

namespace TasksApp.Models
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public AuditEntry()
        {
        }

        public EntityEntry Entry { get; }
        public string UserEmail { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();

        //public Audit GetData(int ID)
        //{
        //    Audit mod = new Audit();
        //    AuditableIdentityContext ent = new AuditableIdentityContext();
        //    Sample rec = ent.samples.FirstOrDefault(s => s.Id == ID);
        //    if (rec != null)
        //    {
        //        mod.Id = rec.Id;
        //        mod.UserEmail = rec.UserEmail;
        //        mod.Type = rec.Type;
        //        mod.TableName = rec.TableName;
        //        mod.DateTime = rec.DateTime;
        //        mod.KeyFieldId = rec.KeyFieldId;
        //        mod.PrimaryKey = rec.PrimaryKey;
        //        mod.OldValues = rec.OldValues;
        //        mod.NewValues = rec.NewValues;
        //        mod.AffectedColumns = rec.AffectedColumns;
        //    }
        //    return mod;
        //}
        public Audit ToAudit(AuditType AuditType, int id, int KeyFieldId, object OldObject, object NewObject)
        {
            CompareLogic compObjects = new CompareLogic();
            compObjects.Config.MaxDifferences = 99;
            ComparisonResult compResult = compObjects.Compare(OldObject, NewObject);
            List<AuditDelta> DeltaList = new List<AuditDelta>();
            foreach (var change in compResult.Differences)
            {
                AuditDelta delta = new AuditDelta();
                if (change.PropertyName.Substring(0, 1) == ".")
                    delta.AffectedColumns = change.PropertyName.Substring(1, change.PropertyName.Length - 1);
                delta.OldValues = change.Object1Value;
                delta.NewValues = change.Object2Value;
                DeltaList.Add(delta);
            }

            var audit = new Audit();
            audit.UserEmail = UserEmail;
            audit.Type = AuditType.ToString();
            audit.TableName = TableName;
            audit.DateTime = DateTime.Now;
            audit.KeyFieldId = KeyFieldId;
            audit.PrimaryKey = JsonConvert.SerializeObject(KeyValues);
            audit.OldValues = JsonConvert.SerializeObject(OldObject);
            audit.NewValues = JsonConvert.SerializeObject(NewObject);
            audit.AffectedColumns = JsonConvert.SerializeObject(ChangedColumns);
            return audit;
        }

        

        public List<AuditChange> GetAudit(int Id)
        {

            List<AuditChange> rslt = new List<AuditChange>();
            //Audit ent = new Audit();
            //AuditableIdentityContext ent = new AuditableIdentityContext();
            //var AuditTrail = ent.AuditLogs.Where(s => s.KeyFieldId == Id).OrderByDescending(s => s.DateTime); // we are looking for audit-history of the record selected.
            var serializer = new XmlSerializer(typeof(AuditDelta));
            //foreach (var record in AuditTrail)
            {
                AuditChange Change = new AuditChange();
                //Change.DateTimeStamp = record.DateTime.ToString();
                //Change.AuditType = (AuditType)record.AuditType;
                //Change.AuditTypeName = Enum.GetName(typeof(AuditType), record.Type);
                List<AuditDelta> delta = new List<AuditDelta>();
                //delta = JsonConvert.DeserializeObject<List<AuditDelta>>(record.AffectedColumns);
                Change.Changes.AddRange(delta);
                rslt.Add(Change);
            }
            return rslt;
        }

        //internal Audit ToAudit()
        //{
        //    throw new NotImplementedException();
        //}

        //internal object ToAudit(int id)
        //{
        //    throw new NotImplementedException();
        //}
    }
    public class AuditChange
    {
        public string DateTimeStamp { get; set; }
        public AuditType AuditType { get; set; }
        public string AuditTypeName { get; set; }
        public List<AuditDelta> Changes { get; set; }
        public AuditChange()
        {
            Changes = new List<AuditDelta>();
        }
    }
    public class AuditDelta
    {
        public string AffectedColumns { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }
}

