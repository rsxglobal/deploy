using Sabio.Data;
using Sabio.Web.Classes;
using Sabio.Web.Domain;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Responses;
using Sabio.Web.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Sabio.Web.Services
{
    public class PersonService : BaseService, IPersonService
    {
        public int Insert(PersonAddRequest model)
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Person_Insert",
                inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    MapCommonParameters(model, paramCollection);

                    SqlParameter outputParam = new SqlParameter("@Id", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;

                    paramCollection.Add(outputParam);
                }, returnParameters: delegate (SqlParameterCollection param)
                {
                    Int32.TryParse(param["@Id"].Value.ToString(), out id);
                }
                );

            return id;
        }

        public void Update(PersonUpdateRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Person_Update",
                inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", model.Id);
                    MapCommonParameters(model, paramCollection);
                }
                );

            return;
        }

        private void MapCommonParameters(PersonAddRequest model, SqlParameterCollection paramCollection)
        {
            paramCollection.AddWithValue("@FirstName", model.FirstName);
            paramCollection.AddWithValue("@MiddleName", model.MiddleName);
            paramCollection.AddWithValue("@LastName", model.LastName);
            paramCollection.AddWithValue("@PhoneNumber", model.PhoneNumber);
            paramCollection.AddWithValue("@Email", model.Email);
            paramCollection.AddWithValue("@JobTitle", model.JobTitle);
            paramCollection.AddWithValue("@DateOfBirth", model.DateOfBirth);
            paramCollection.AddWithValue("@Address1", model.Address1);
            paramCollection.AddWithValue("@Address2", model.Address2);
            paramCollection.AddWithValue("@City", model.City);
            paramCollection.AddWithValue("@StateProvinceId", model.StateProvinceId);
            paramCollection.AddWithValue("@PostalCode", model.PostalCode);
            paramCollection.AddWithValue("@CountryId", model.CountryId);
            paramCollection.AddWithValue("@IsVeteran", model.IsVeteran);
            paramCollection.AddWithValue("@IsEmployer", model.IsEmployer);
            paramCollection.AddWithValue("@IsFamilyMember", model.IsFamilyMember);
            paramCollection.AddWithValue("@Description", model.Description);
            paramCollection.AddWithValue("@EmploymentStatus", model.EmploymentStatus);
            paramCollection.AddWithValue("@IsEmployed", model.IsEmployed);
            paramCollection.AddWithValue("@OpCodeEmployAssist", model.OpCodeEmployAssist);
            paramCollection.AddWithValue("@EducationLevelId", model.EducationLevelId);

            //start of Language
            if (model.LanguageProficiencyKeys != null)
            {
                SqlParameter plang = new SqlParameter("@LanguageProficiencyKeys", SqlDbType.Structured);
                if (model.LanguageProficiencyKeys.Any())
                {
                    plang.Value = new NVarcharTable(model.LanguageProficiencyKeys);
                }
                paramCollection.Add(plang);
            }
            //end of Language
            //start of Skill
            DataTable SkillIdArray = new DataTable();
            SkillIdArray.Columns.Add("SkillIds", typeof(Int32));
            if (model.SkillIds != null)
            {
                for (int i = 0; i < model.SkillIds.Count; i++)
                {
                    SkillIdArray.Rows.Add(model.SkillIds[i]);
                }
            }
            SqlParameter SkillIdTable = new SqlParameter();
            SkillIdTable.ParameterName = "@PersonSkillIds";
            SkillIdTable.SqlDbType = System.Data.SqlDbType.Structured;
            SkillIdTable.Value = SkillIdArray;
            paramCollection.Add(SkillIdTable);
            //end of Skill //
            //Start of Base//
            DataTable MilitaryBaseIdArray = new DataTable();
            MilitaryBaseIdArray.Columns.Add("MilitaryBaseId", typeof(Int32));
            if (model.MilitaryBaseIds != null)
            {
                for (int i = 0; i < model.MilitaryBaseIds.Count; i++)
                {
                    MilitaryBaseIdArray.Rows.Add(model.MilitaryBaseIds[i]);
                }
            }
            SqlParameter MilitaryBaseIdTable = new SqlParameter();
            MilitaryBaseIdTable.ParameterName = "@PersonMilitaryBaseIds";
            MilitaryBaseIdTable.SqlDbType = System.Data.SqlDbType.Structured;
            MilitaryBaseIdTable.Value = MilitaryBaseIdArray;
            paramCollection.Add(MilitaryBaseIdTable);
            //End of Base//
            //Start of Notifications//
            DataTable PersonNotificationPreferenceArray= new DataTable();
            PersonNotificationPreferenceArray.Columns.Add("PersonId", typeof(Int32));
            PersonNotificationPreferenceArray.Columns.Add("NotificationEventId", typeof(Int32));
            PersonNotificationPreferenceArray.Columns.Add("SendEmail", typeof(bool));
            PersonNotificationPreferenceArray.Columns.Add("SendText", typeof(bool));

            if (model.Preferences != null)
            {
                for (int i = 0; i < model.Preferences.Length; i++)
                {
                    DataRow dr = PersonNotificationPreferenceArray.NewRow();
                    dr["PersonId"] = model.Preferences[i].PersonId;
                    dr["NotificationEventId"] = model.Preferences[i].NotificationEventId;
                    dr["SendEmail"] = model.Preferences[i].SendEmail;
                    dr["SendText"] = model.Preferences[i].SendText;
                    PersonNotificationPreferenceArray.Rows.Add(dr);
                }
            }
            SqlParameter NotificationEventTable = new SqlParameter();
            NotificationEventTable.ParameterName = "@PersonNotificationPreferences";
            NotificationEventTable.SqlDbType = System.Data.SqlDbType.Structured;
            NotificationEventTable.Value = PersonNotificationPreferenceArray;
            paramCollection.Add(NotificationEventTable);
        }
        public void Delete(int id)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Person_Delete",
                inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@Id", id);
                }
                );
        }

        public Person SelectById(int id)
        {
            Person person = null;
            List<MilitaryBase> mbList = null;
            List<PersonLanguage> laList = null;
            List<Skill> skList = null;
           

            DataProvider.ExecuteCmd(GetConnection, "dbo.Person_SelectById",
               inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@Id", id);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   switch (set)
                   {
                       case 0:
                           person = MapPerson(reader);
                           break;

                       case 1:
                           int personId = 0;
                           MilitaryBase mb = MapMilitaryBaseList(reader, out personId);
                           if (mbList == null)
                           {
                               mbList = new List<MilitaryBase>();
                           }
                           mbList.Add(mb);
                           break;

                       case 2:
                           PersonLanguage la = MapLanguageList(reader, out personId);
                           if (laList == null)
                           {
                               laList = new List<PersonLanguage>();
                           }
                           laList.Add(la);
                           break;
                       case 3:
                           Skill sk = MapSkillList(reader, out personId);
                           if (skList == null)
                           {
                               skList = new List<Skill>();
                           }
                           skList.Add(sk);
                           break;
                       case 4:
                           CompanyBase cb = MapCompany(reader, out personId);
                           if (person.Companies == null)
                           {
                               person.Companies = new List<CompanyBase>();
                           }
                           person.Companies.Add(cb);
                           break;
                       case 5:
                           SquadBase sq = MapSquad(reader, out personId);
                           if (person.Squads == null)
                           {
                               person.Squads = new List<SquadBase>();
                           }
                           person.Squads.Add(sq);
                           break;
                       case 6:
                           PersonNotificationPreference pnp = PreferenceMap(reader, out personId);
                           if (person.Preferences == null)
                           {
                               person.Preferences = new List<PersonNotificationPreference>();
                           }
                           person.Preferences.Add(pnp);
                           break;
                   }
               }
             );
            person.MilitaryBases = mbList;
            person.Languages = laList;
            person.Skills = skList;
            return person;
        }

        private SquadBase MapSquad(IDataReader reader, out int personId)
        {
            SquadBase sq = new SquadBase();
            int startingIndex = 0; //startingOrdinal

            personId = reader.GetSafeInt32(startingIndex++);
            sq.Id = reader.GetSafeInt32(startingIndex++);
            sq.IsLeader = reader.GetSafeBool(startingIndex++);
            sq.Name = reader.GetSafeString(startingIndex++);
            sq.Mission = reader.GetSafeString(startingIndex++);
            

            return sq;
        }

        private CompanyBase MapCompany(IDataReader reader, out int personId)
        {
            CompanyBase company = new CompanyBase();
            int startingIndex = 0; //startingOrdinal

            personId = reader.GetSafeInt32(startingIndex++);
            company.Id = reader.GetSafeInt32(startingIndex++);
            company.Name = reader.GetSafeString(startingIndex++);
            company.PhoneNumber = reader.GetSafeString(startingIndex++);
            company.Email = reader.GetSafeString(startingIndex++);

            return company;
        }

        private PersonNotificationPreference PreferenceMap(IDataReader reader, out int personId)
        {
            PersonNotificationPreference pnp = new PersonNotificationPreference();
            int ord = 0;

            
            personId = reader.GetSafeInt32(ord++);
            pnp.NotificationEventId = reader.GetSafeInt32(ord++);
            pnp.NotificationEventName = reader.GetSafeString(ord++);
            pnp.SendEmail = reader.GetSafeBool(ord++);
            pnp.SendText = reader.GetSafeBool(ord++);

            return pnp;
        }

        public PersonLayout GetByAspNetUserId(string AspNetUserId)
        {
            PersonLayout pLayout = null;
            TimecardEntryBase teb = null;
            List<ProjectBase> projects = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Person_SelectByAspNetUserId",
               inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@AspNetUserId", AspNetUserId);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   switch (set)
                   {
                       case 0:
                           pLayout = new PersonLayout();
                           int ord = 0;

                           pLayout.Id = reader.GetSafeInt32(ord++);
                           pLayout.FirstName = reader.GetSafeString(ord++);
                           pLayout.MiddleName = reader.GetSafeString(ord++);
                           pLayout.LastName = reader.GetSafeString(ord++);
                           pLayout.PhoneNumber = reader.GetSafeString(ord++);
                           pLayout.Email = reader.GetSafeString(ord++);
                           pLayout.JobTitle = reader.GetSafeString(ord++);
                           break;

                       case 1:

                           ProjectBase pb = MapProjectList(reader);
                           if (projects == null)
                           {
                               projects = new List<ProjectBase>();
                           }
                           projects.Add(pb);
                           break;

                       case 2:
                           teb = new TimecardEntryBase();
                           int ordinal = 0;

                           teb.Id = reader.GetSafeInt32(ordinal++);
                           teb.ProjectId = reader.GetSafeInt32(ordinal++);
                           teb.StartDateTimeUtc = reader.GetSafeDateTimeNullable(ordinal++);
                           teb.EndDateTimeUtc = reader.GetSafeDateTimeNullable(ordinal++);
                          
                           break;
                   }
               }
             );
            pLayout.Projects = projects;
            pLayout.TimecardEntry = teb;
            return pLayout;
        }

        public int InsertFromRegister(PersonAddRequest model)
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Person_Insert",
                inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@FirstName", model.FirstName);
                    paramCollection.AddWithValue("@LastName", model.LastName);
                    paramCollection.AddWithValue("@Email", model.Email);
                    paramCollection.AddWithValue("@AspNetUserId", model.AspNetUserId);                 
                    SqlParameter outputParam = new SqlParameter("@Id", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;

                    paramCollection.Add(outputParam);
                }, returnParameters: delegate (SqlParameterCollection param)
                {
                    Int32.TryParse(param["@Id"].Value.ToString(), out id);
                }
                );

            return id;
        }

        private Person MapPerson(IDataReader reader)
        {
            Person person = new Person();
            int startingIndex = 0; //startingOrdinal

            person.Id = reader.GetSafeInt32(startingIndex++);
            person.FirstName = reader.GetSafeString(startingIndex++);
            person.MiddleName = reader.GetSafeString(startingIndex++);
            person.LastName = reader.GetSafeString(startingIndex++);
            person.PhoneNumber = reader.GetSafeString(startingIndex++);
            person.Email = reader.GetSafeString(startingIndex++);
            person.JobTitle = reader.GetSafeString(startingIndex++);
            person.DateOfBirth = reader.GetSafeDateTimeNullable(startingIndex++);
            person.Address1 = reader.GetSafeString(startingIndex++);
            person.Address2 = reader.GetSafeString(startingIndex++);
            person.City = reader.GetSafeString(startingIndex++);
            int stateProvinceId = reader.GetSafeInt32(startingIndex++);
            if (stateProvinceId > 0)
            {
                person.StateProvince = new StateProvinceBase();
                person.StateProvince.Id = stateProvinceId;
                person.StateProvince.Code = reader.GetSafeString(startingIndex++);
                person.StateProvince.Name = reader.GetSafeString(startingIndex++);
            }
            else
            {
                person.StateProvince = new StateProvinceBase();
                startingIndex += 2;
            }
            person.PostalCode = reader.GetSafeString(startingIndex++);

            int countryId = reader.GetSafeInt32(startingIndex++);
            if (countryId > 0)
            {
                person.Country = new Country();
                person.Country.Id = countryId;
                person.Country.LongCode = reader.GetSafeString(startingIndex++);
                person.Country.Code = reader.GetSafeString(startingIndex++);
                person.Country.Name = reader.GetSafeString(startingIndex++);
            }
            else
            {
                person.Country = new Country();
                startingIndex += 3;
            }
            person.AspNetUserId = reader.GetSafeString(startingIndex++);
            person.PhotoKey = reader.GetSafeString(startingIndex++);
            person.ProfilePicture = SiteConfig.GetUrlFromFileKey(person.PhotoKey);
            person.IsVeteran = reader.GetSafeBool(startingIndex++);
            person.IsEmployer = reader.GetSafeBool(startingIndex++);
            person.IsFamilyMember = reader.GetSafeBool(startingIndex++);
            person.Description = reader.GetSafeString(startingIndex++);
            person.EmploymentStatus = reader.GetSafeString(startingIndex++);
            person.IsEmployed = reader.GetSafeBool(startingIndex++);
            person.OpCodeEmployAssist = reader.GetSafeBool(startingIndex++);
            person.EducationLevelId = reader.GetSafeInt32(startingIndex++);

            return person;
        }

        public List<Person> GetAll()
        {
            List<Person> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Person_SelectAll"
               , inputParamMapper: null
               , map: delegate (IDataReader reader, short set)
               {
                   switch (set)
                   {
                       case 0:
                           Person p = MapPerson(reader);

                           if (list == null)
                           {
                               list = new List<Person>();
                           }
                           list.Add(p);
                           break;

                       case 1:
                           int personId = 0;
                           MilitaryBase mb = MapMilitaryBaseList(reader, out personId);
                           Person person = list.Find(item => item.Id == personId);
                           if (person.MilitaryBases == null)
                           {
                               person.MilitaryBases = new List<MilitaryBase>();
                           }
                           person.MilitaryBases.Add(mb);
                           break;

                       case 2:
                           int personaId = 0;
                           PersonLanguage bl = MapLanguageList(reader, out
                                personaId);
                           Person persona = list.Find(item => item.Id == personaId);
                           if (persona != null)
                           {
                               if (persona.Languages == null)
                               {
                                   persona.Languages = new List<PersonLanguage>();
                               }
                               persona.Languages.Add(bl);
                           }
                           break;

                       case 3:
                           int personalId = 0;
                           Skill sk = MapSkillList(reader, out personalId);
                           Person personal = list.Find(item => item.Id == personalId);
                           if (personal != null)
                           {
                               if (personal.Skills == null)
                               {
                                   personal.Skills = new List<Skill>();
                               }
                               personal.Skills.Add(sk);
                           }

                           break;
                       default:
                           break;
                   }
               }
               );
            return list;
        }

        private static Skill MapSkillList(IDataReader reader, out int personId)
        {
            Skill sk = new Skill();
            int ord = 0;
            personId = reader.GetSafeInt32(ord++);
            sk.Id = reader.GetSafeInt32(ord++);
            sk.Name = reader.GetSafeString(ord++);
            //sk.Code = reader.GetSafeString(ord++);
            return sk;
        }

        private static PersonLanguage MapLanguageList(IDataReader reader, out int personId)
        {
            PersonLanguage bl = new PersonLanguage();
            int ord = 0;
            personId = reader.GetSafeInt32(ord++);
            bl.Key = reader.GetSafeString(ord++);
            bl.LanguageId = reader.GetSafeInt32(ord++);
            bl.LanguageName = reader.GetSafeString(ord++);
            bl.LanguageProficiencyId = reader.GetSafeInt32(ord++);
            bl.LanguageProficiencyCode = reader.GetSafeString(ord++);
            return bl;
        }

        private static MilitaryBase MapMilitaryBaseList(IDataReader reader, out int personId)
        {
            MilitaryBase mb = new MilitaryBase();
            int ord = 0;
            personId = reader.GetSafeInt32(ord++);
            mb.Id = reader.GetSafeInt32(ord++);
            mb.MilitaryBaseName = reader.GetSafeString(ord++);
            mb.ServiceBranchId = reader.GetSafeInt32(ord++);
            mb.ServiceBranchName = reader.GetSafeString(ord++);
            return mb;
        }

        private static ProjectBase MapProjectList(IDataReader reader)
        {
            ProjectBase pb = new ProjectBase();
            int ord = 0;
            pb.Id = reader.GetSafeInt32(ord++);
            pb.ProjectName = reader.GetSafeString(ord++);
            return pb;
        }

        public bool CheckIfPerson(string aspNetUserId)
        {
            bool personExists = false;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Person_CheckByAspNetUserId",
               inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@AspNetUserId", aspNetUserId);
                   SqlParameter outputParam = new SqlParameter("@Exists", SqlDbType.Bit);
                   outputParam.Direction = ParameterDirection.Output;
                   paramCollection.Add(outputParam);
               }, returnParameters: delegate (SqlParameterCollection param)
               {
                   bool.TryParse(param["@Exists"].Value.ToString(), out personExists);
               }
               );
            return personExists;
        }

        public List<PersonBase> PersonBaseSearch(PersonSearchRequest model, out int totalRows)
        {
            List<PersonBase> list = null;
            
            int r = 0;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Person_Search",
               inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@SearchStr", model.SearchString);
                   paramCollection.AddWithValue("@CurrentPage", model.CurrentPage);
                   paramCollection.AddWithValue("@ItemsPerPage", model.ItemsPerPage);
                   paramCollection.AddWithValue("@Latitude", model.Latitude);
                   paramCollection.AddWithValue("@Longitude", model.Longitude);
                   paramCollection.AddWithValue("@Radius", model.Radius);
                   paramCollection.AddWithValue("@IsVeteran", model.IsVeteran);
                   paramCollection.AddWithValue("@IsEmployer", model.IsEmployer);
                   paramCollection.AddWithValue("@IsFamilyMember", model.IsFamilyMember);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   switch (set)
                   {
                       case 0:

                           Person p = new Person();
                           p.StateProvince = new StateProvince();
                           p.Country = new Country();
                           int ord = 0; //startingOrdinal

                           p.Id = reader.GetSafeInt32(ord++);
                           p.FirstName = reader.GetSafeString(ord++);
                           ord++;
                           p.LastName = reader.GetSafeString(ord++);
                           p.PhoneNumber = reader.GetSafeString(ord++);
                           p.Email = reader.GetSafeString(ord++);
                           p.JobTitle = reader.GetSafeString(ord++);
                           p.PhotoKey = reader.GetSafeString(ord++);
                           p.IsVeteran = reader.GetSafeBool(ord++);
                           p.IsEmployer = reader.GetSafeBool(ord++);
                           p.IsFamilyMember = reader.GetSafeBool(ord++);
                           p.ProfilePicture = SiteConfig.GetUrlFromFileKey(p.PhotoKey);                         
                           p.Address1 = reader.GetSafeString(ord++);
                           p.Address2 = reader.GetSafeString(ord++);
                           p.City = reader.GetSafeString(ord++);
                           p.StateProvince.Id = reader.GetSafeInt32(ord++);
                           p.StateProvince.Name = reader.GetSafeString(ord++);
                           p.Country.Id = reader.GetSafeInt32(ord++);
                           p.Country.Name = reader.GetSafeString(ord++);
                           p.Latitude = reader.GetSafeDecimal(ord++);
                           p.Longitude = reader.GetSafeDecimal(ord++);
                           r = reader.GetSafeInt32(ord++);

                           if (list == null)
                           {
                               list = new List<PersonBase>();
                           }

                           list.Add(p);
                           break;
                   }
               }
               );
            totalRows = r;
            return list;
        }

        public List<PersonProject> SelectProjectByPersonId(int id)
        {
            List<PersonProject> ppList = null;
            PersonProject pp = new PersonProject();

            DataProvider.ExecuteCmd(GetConnection, "dbo.PersonProject_SelectByPersonId",
               inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@PersonId", id);
               }
               , map: delegate (IDataReader reader, short set)
               {
                   switch (set)
                   {
                       case 0:
                           pp = MapPersonProject(reader);
                           if (ppList == null)
                           {
                               ppList = new List<PersonProject>();
                           }
                           ppList.Add(pp);
                           break;         
                   }
               }
             );
            return ppList;
        }

        private PersonProject MapPersonProject(IDataReader reader)
        {
            PersonProject pp = new PersonProject();
            ProjectBase pb = new ProjectBase();            
            CompanyBase cb = new CompanyBase();

            pp.Project = pb;
            pp.Company = cb; 
            int startingIndex = 0; //startingOrdinal

            pp.PersonId = reader.GetSafeInt32(startingIndex++);
            pb.Id = reader.GetSafeInt32(startingIndex++);
            pb.ProjectName = reader.GetSafeString(startingIndex++);
            pp.Deadline = reader.GetSafeDateTime(startingIndex++);
            pp.IsLeader = reader.GetSafeBool(startingIndex++);
            pp.ProjectPersonStatusId = reader.GetSafeInt32(startingIndex++);
            pp.ProjectPersonStatus = reader.GetSafeString(startingIndex++);            
            cb.Id = reader.GetSafeInt32(startingIndex++);
            cb.Name = reader.GetSafeString(startingIndex++);
            cb.PhoneNumber = reader.GetSafeString(startingIndex++);
            cb.Email = reader.GetSafeString(startingIndex++);
            pp.CurrentRate = reader.GetSafeDecimalNullable(startingIndex++);
            pp.TrelloBoardUrl = reader.GetSafeString(startingIndex++);
            pp.TimeWorked = reader.GetSafeInt32Nullable(startingIndex++);
            pp.Earnings = reader.GetSafeDecimalNullable(startingIndex++);

            return pp;
        }
    }
}
