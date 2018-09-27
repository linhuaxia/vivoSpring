using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Vivo.Model;
using Vivo.IBLL;
using Vivo.BLLFactory;
using Tool;
using System.Threading;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;

namespace Vivo.web.Areas.MP.Controllers
{
    public class UserController : BaseMPController
    {

        public ActionResult Index(int page = 1)
        {
            GetSelectList();
            IQueryable<UserInfo> list = GetDataList();
            var result = list.ToPagedList(page, PageSize);
            return View(result);
        }
        private void GetSelectList()
        {
            ViewBag.listRule = RuleBLL.GetList(a => a.Enable).Select(p => new SelectListItem() { Text = p.Name, Value = p.ID.ToString() }).ToList();
            ViewBag.listMemberGroup = MemberGroupBLL.GetList(a => a.Enable).Select(p => new SelectListItem() { Text = p.Name, Value = p.ID.ToString() }).ToList();
            IEnumerable<SelectListItem> listDepartmentID = DepartmentBLL.GetList(p => p.Enable == true).OrderBy(a=>a.Name).Select(p => new SelectListItem { Text = p.Name, Value = p.ID.ToString() }).ToList();
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_系统管理.PP系统帐户管理.PPP用户管理.查看所有帐户))
            {
                listDepartmentID = listDepartmentID.Where(a => a.Value == CurrentUser.DepartmentID.ToString());
            }
            ViewBag.listDepartment = listDepartmentID;
            IEnumerable<SelectListItem> listEnableStatus = Common.EnumToSelectListItem(typeof(SysEnum.EnableStatus.EnumEnableStatus));
            ViewBag.EnableStatus = listEnableStatus;

        }


        private IQueryable<UserInfo> GetDataList()
        {
            IQueryable<UserInfo> list = UserBLL.GetList(p => true);

            int PowerGroupID = Function.GetRequestInt("PowerGroupID");
            int DepartmentID = Function.GetRequestInt("DepartmentID");
            int EnableStatus = Function.GetRequestInt("EnableStatus");
            int SubjectID = Function.GetRequestInt("SubjectID");
            string Name = Function.GetRequestString("Name");

            if (!PowerActionBLL.PowerCheck(PowerInfo.P_系统管理.PP系统帐户管理.PPP用户管理.查看所有帐户))
            {
                DepartmentID = CurrentUser.DepartmentID;
                if (!PowerActionBLL.PowerCheck(PowerInfo.P_系统管理.PP系统帐户管理.PPP用户管理.查看当前单位帐户))
                {
                    list = list.Where(a => a.ID == CurrentUser.ID);
                }
            }


            if (PowerGroupID > 0)
            {
                list = list.Where(p => p.RuleInfo.Any(r => r.ID == PowerGroupID));
                ViewBag.DdlPowerGroupID = PowerGroupID;
            }
            if (DepartmentID > 0)
            {
                list = list.Where(p => p.DepartmentID == DepartmentID);
                ViewBag.DdlDepartmentID = DepartmentID;
            }

            if (EnableStatus > 0)
            {
                list = list.Where(p => p.Enable == (EnableStatus == (int)SysEnum.EnableStatus.EnumEnableStatus.启用));
                ViewBag.DdlEnableStatus = EnableStatus;
            }
            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(p => p.Name.Contains(Name)
                 || p.WechatNameNick.Contains(Name)
                 || p.IDCard.Contains(Name)
                 || p.Code.Contains(Name));
                ViewBag.TxtName = Name;
            }
            if (SubjectID > 0)
            {
                list = list.Where(p => p.TypeID == SubjectID);
                ViewBag.DdlSubjectID = SubjectID;
            }
            

            list = list.OrderByDescending(p => p.Enable)
                .ThenByDescending(p=>p.DepartmentID)
                .ThenByDescending(p => p.ID);


            return list;
        }

        public ActionResult Export()
        {
            IQueryable<UserInfo> list = GetDataList();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("登录名");
            dt.Columns.Add("真实姓名");
            dt.Columns.Add("单位名称");
            dt.Columns.Add("是否启用");
            dt.Columns.Add("微信OpenID");
            dt.Columns.Add("微信昵称");
            dt.Columns.Add("创建时间");
            dt.Columns.Add("角色");
            dt.Columns.Add("学科");

            foreach (var item in list)
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = item.ID;
                dr["登录名"] = item.Code;
                dr["真实姓名"] = item.Name;
                dr["单位名称"] = item.DepartmentInfo.Name;
                dr["是否启用"] = item.Enable ? "可用" : "x";
                dr["微信OpenID"] = item.WechatOpenID;
                dr["微信昵称"] = item.WechatNameNick;
                dr["创建时间"] = item.CreateDate;
                dr["角色"] = string.Join("/", item.RuleInfo.Select(r => r.Name));
                dr["学科"] = string.Join("/", item.SubjectInfo.Select(r => r.Name));
                dt.Rows.Add(dr);
            }
            string FilePath = "/Content/Export/User/";
            string FileFullPath = Server.MapPath(FilePath);
            if (!Directory.Exists(FileFullPath))
            {
                Directory.CreateDirectory(FileFullPath);

            }
            string FileName = DateTime.Now.ToFileTime().ToString() + ".xlsx";
            NPOIHelper.Export(dt, FileFullPath + FileName);
            return Redirect(FilePath + FileName);
        }

        public ActionResult Create()
        {
            GetSelectList();
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(UserInfo info)
        {

            UserInfo infoExist = UserBLL.GetList(p => p.Name == info.Name || p.Code == info.Code).FirstOrDefault();

            if (info.DepartmentID <= 0)
            {
                return Json(new APIJson("请选择所在部门"));
            }
            if (null != infoExist)
            {
                return Json(new APIJson("登录名和姓名重复，请起用其它名字"));
            }
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json(new APIJson("姓名不能为空"));
            }
            if (string.IsNullOrEmpty(info.Code))
            {
                info.Code = info.Name;
            }
            if (!string.IsNullOrEmpty(info.PassWord))
            {
                bool IsPassWordValidate = ValidatePassWord(info);
                if (!IsPassWordValidate)
                {
                    return Json(new APIJson("密码必需包含数字、字母，并且长度在8到16位"));
                }
            }
            else
            {
                info.PassWord = "lw123456";
            }

            if (string.IsNullOrEmpty(info.Email))
            {
                info.Email = string.Empty;
            }
            if (string.IsNullOrEmpty(info.Tel))
            {
                info.Tel = string.Empty;
            }
            if (string.IsNullOrEmpty(info.IDCard))
            {
                info.IDCard = string.Empty;
            }


            info.PassWord = Tool.Md5Helper.Md5(info.PassWord);
            info.CreateDate = DateTime.Now;
            info.LastDate = DicInfo.DateZone;
            info.LocationX = info.LocationY = 0;
            info.WechatOpenID = string.Empty;
            info.WechatNameNick = string.Empty;
            info.WechatHeadImg = string.Empty;
            info.Sex = string.Empty;
            if (info.DefaultSubjectID>0)
            {
                SubjectInfo infoSubject = SubjectBLL.GetList(a => a.ID == info.DefaultSubjectID).FirstOrDefault();
                if (null!=infoSubject)
                {
                    info.SubjectInfo.Add(infoSubject);
                }
            }
            info = UserBLL.Create(info);
            if (info.ID > 0)
            {
                return Json(new APIJson(0, "添加成功", new { ID = info.ID,Name=info.Name }));
            }
            return Json(new APIJson(-1, "添加失败"));
        }

        public static bool ValidatePassWord(UserInfo info)
        {
            //var regex = new Regex(@"
            //                    (?=.*[0-9])                     #必须包含数字
            //                    (?=.*[a-zA-Z])                  #必须包含小写或大写字母
            //                    (?=([\x21-\x7e]+)[^a-zA-Z0-9])  #必须包含特殊符号
            //                    .{6,16}                         #至少8个字符，最多30个字符
            //                    ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            var regex = new Regex(@"
                                (?=.*[0-9])                     #必须包含数字
                                (?=.*[a-zA-Z])                  #必须包含小写或大写字母
                                ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            if (string.IsNullOrEmpty(info.PassWord))
            {
                return false;
            }
            bool IsPassWordValidate = regex.IsMatch(info.PassWord);
            return IsPassWordValidate;
        }

        //public static bool ValidateEmail(UserInfo info)
        //{
        //    var regex = new Regex(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+$", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        //    bool IsEmailValidate = regex.IsMatch(info.Email);
        //    return IsEmailValidate;
        //}

        public ActionResult Edit(int id)
        {
            UserInfo info = UserBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson("参数有误"));
            }

            GetSelectList();

            return View(info);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(UserInfo info)
        {
            UserInfo infoExist = UserBLL.GetList(p => p.ID != info.ID && (p.Name == info.Name || p.Code == info.Code)).FirstOrDefault();
            if (info.DepartmentID <= 0)
            {
                return Json(new APIJson("请选择所在学校部门"));
            }
            if (null != infoExist)
            {
                return Json(new APIJson("登录名和姓名不能与其它用户重复"));
            }
            if (string.IsNullOrEmpty(info.Name))
            {
                return Json(new APIJson("姓名不能为空"));
            }
            if (string.IsNullOrEmpty(info.Code))
            {
                return Json(new APIJson("登录名不能为空"));
            }
            if (string.IsNullOrEmpty(info.Tel))
            {
                info.Tel = string.Empty;
            }
            if (string.IsNullOrEmpty(info.IDCard))
            {
                info.IDCard = string.Empty;
            }
            if (string.IsNullOrEmpty(info.Sex))
            {
                info.Sex = "未知";
            }

            UserInfo infoDB = UserBLL.GetList(p => p.ID == info.ID).FirstOrDefault();

            if (!string.IsNullOrEmpty(info.PassWord))
            {
                bool IsPassWordValidate = ValidatePassWord(info);
                if (!IsPassWordValidate)
                {
                    return Json(new APIJson("密码必需包含数字、字母，并且长度在8到16位"));
                }
                infoDB.PassWord = Tool.Md5Helper.Md5(info.PassWord);
            }
            if (string.IsNullOrEmpty(info.Email))
            {
                info.Email = string.Empty;
            }
            if (string.IsNullOrEmpty(info.Tel))
            {
                info.Tel = string.Empty;
            }


            infoDB.DepartmentID = info.DepartmentID;
            infoDB.Name = info.Name;
            infoDB.Code = info.Code;
            infoDB.Sex = info.Sex;
            infoDB.Email = info.Email;
            infoDB.Tel = info.Tel;
            infoDB.Enable = info.Enable;
            infoDB.TypeID = info.TypeID;
            infoDB.IDCard = info.IDCard;

            if (UserBLL.Edit(infoDB))
            {
                return Json(new APIJson(0, "提交成功", new { ID = infoDB.ID }));
            }
            return Json(new APIJson("提交失败"));
        }

        public ActionResult Delete(int id)
        {
            UserInfo info = UserBLL.GetList(p => p.ID == id).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "删除失败，参数有误", info));
            }
            if (UserBLL.Delete(info))
            {
                return Json(new APIJson(0, "删除成功", info));

            }
            return Json(new APIJson(-1, "删除失败，请重试", info));
        }
        public ActionResult Pwd()
        {
            UserInfo info = UserBLL.GetCurrent();
            return View(info);
        }

        [HttpPost]
        public ActionResult Pwd(UserInfo info)
        {
            info.PassWord = Function.GetRequestString("PwdNew");
            UserInfo infoCurrent = UserBLL.GetCurrent();
            infoCurrent = UserBLL.GetList(p => p.ID == infoCurrent.ID).FirstOrDefault();
            if (info.ID != infoCurrent.ID)
            {
                return Json(new APIJson(-1, "数据错误"));
            }
            string PwdOld = Function.GetRequestString("PwdOld");
            if (!PowerActionBLL.PowerCheck(PowerInfo.P_系统管理.PP系统帐户管理.PPP用户管理.重置任意人员密码))
            {
                if (Tool.Md5Helper.Md5(PwdOld) != infoCurrent.PassWord)
                {
                    return Json(new APIJson(-1, "原始密码错误"));
                }
            }
            if (!ValidatePassWord(info))
            {
                return Json(new APIJson(-1, "密码必需包含数字、字母，并且长度在8到16位"));
            }
            infoCurrent.PassWord = Tool.Md5Helper.Md5(info.PassWord);
            if (UserBLL.Edit(infoCurrent))
            {
                return Json(new APIJson(0, "密码修改成功，请使用新密码重新登录。"));
            }
            return Json(new APIJson(-1, "失败了，请重试"));
        }

        [HttpPost]
        public ActionResult DeleteWechatOpenID(int ID)
        {
            UserInfo info = UserBLL.GetList(p => p.ID == ID).FirstOrDefault();
            if (null == info)
            {
                return Json(new APIJson(-1, "用户不存在"));
            }
            // MP.BLL.WechatService.WechatUser.SetUserRemark(info.WechatOpenID, "已解除绑定");
            info.WechatOpenID = info.WechatNameNick = info.WechatHeadImg = string.Empty;
            if (UserBLL.Edit(info))
            {
                return Json(new APIJson(0, "提交成功"));
            }
            return Json(new APIJson("提交失败"));

        }


        #region Inport

        [AllowAnonymous]
        public ActionResult Inport()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult InportPost1()
        {
            HttpPostedFileBase FileMain = Request.Files["FileMain"];
            if (null == FileMain)
            {
                return Json(new APIJson(-1, "请选择要导入的excel文档"));
            }
            if (FileMain.ContentType != "application/vnd.ms-excel" && FileMain.ContentType != "application/octet-stream")
            {
                return Json(new APIJson(-1, "文件无法识别，请上传97-2003格式"));
            }

            string RelativePath = string.Format("/Content/Export/User/{0}/", DateTime.Now.ToString("yyyyMM"));
            string SaveMapPath = Server.MapPath(RelativePath);
            if (!Directory.Exists(SaveMapPath))
            {
                Directory.CreateDirectory(SaveMapPath);
            }
            string SaveName = DateTime.Now.ToFileTime().ToString() + ".xls";
            FileMain.SaveAs(SaveMapPath + SaveName);
            List<string> listName = new List<string>();
            try
            {
                listName = Tool.ExcelHelper.GetExcelTableName(SaveMapPath + SaveName);
            }
            catch (Exception ex)
            {
                return Json(new APIJson(-1, "系统无法解析这个文档。请确定是不是一个标准的excel 2003文档。错误信息请参考" + ex.Message));
            }
            var result = new
            {
                FileURL = RelativePath + SaveName,
                sheets = listName
            };

            return Json(new APIJson(0, "文档解析成功，请选择要导入的工作簿", result));
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult InportPost2()
        {
            bool HidenIsInport = Function.GetRequestString("HidenIsInport").ToLower() == "true";
            string DdlWorkSheet = Function.GetRequestString("DdlWorkSheet");
            string HidenFileURL = Function.GetRequestString("HidenFileURL");
            int ActivityID = Function.GetRequestInt("ActivityID");
            HidenFileURL = Server.MapPath(HidenFileURL);
            DataTable dt = new DataTable();

            try
            {
                dt = Tool.ExcelHelper.ExcelToDatatable(HidenFileURL, DdlWorkSheet);
            }
            catch (Exception ex)
            {
                return Json(new APIJson(-1, "系统出错:" + ex.Message));
            }
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(dt.Rows[i][0].ToString().Trim()))
                {
                    dt.Rows.RemoveAt(i);
                }
            }


            var ErrorMsg = CheckDataTableHead(dt);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                return Json(new APIJson(-1, ErrorMsg));
            }
            if (!HidenIsInport)
            {
                return Json(new APIJson(dt.DataTableToList()));
            }
            var ListSubject = SubjectBLL.GetList(a => true).ToList();
            var ListRule = RuleBLL.GetList(a => true).ToList();
            List<string> listInportResult = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                var HelpText1 = item["登录名"].ToString().Trim();
                UserInfo info = UserBLL.GetList(a => a.Code == HelpText1).FirstOrDefault();
                if (null == info)
                {
                    info = new UserInfo();
                    info.PassWord = Md5Helper.Md5("qwe123");
                    info.Email = string.Empty;
                    info.Tel = string.Empty;
                    info.CreateDate = DateTime.Now;
                    info.LastDate = DateTime.Now;
                    info.LocationX = info.LocationY = 0;
                    info.WechatOpenID = info.WechatNameNick = info.WechatHeadImg = string.Empty;
                    info.Sex = "未知";
                    info.IDCard = string.Empty;
                    info.TypeID = 0;
                    info.DefaultSubjectID = 0;

                }
                HelpText1 = item["单位名称"].ToString().Trim();
                DepartmentInfo infoDepartment = DepartmentBLL.GetList(a => a.Name == HelpText1).FirstOrDefault();
                if (null == infoDepartment)
                {
                    return Json(new APIJson(-1, string.Format("提交失败，ID:{0}所在行中，单位名称在系统不存在", item["ID"].ToString())));
                }
                info.DepartmentID = infoDepartment.ID;
                info.Enable = item["是否启用"].ToString().Trim() == "可用";
                if (info.ID > 0)
                {
                    if (item["学科"].ToString().Trim() != "忽略")
                    {
                        info.SubjectInfo.Clear();
                    }
                    if (item["角色"].ToString().Trim() != "忽略")
                    {
                        info.RuleInfo.Clear();
                    }
                }
                foreach (var itemSubject in ListSubject.Where(a => item["学科"].ToString().Trim().Split('/').Contains(a.Name)))
                {
                    info.SubjectInfo.Add(itemSubject);
                }
                foreach (var itemRule in ListRule.Where(a => item["角色"].ToString().Trim().Split('/').Contains(a.Name)))
                {
                    info.RuleInfo.Add(itemRule);
                }

                listInportResult.Add(string.Format("ID:{0}验证成功", item["ID"].ToString()));
                if (info.ID == 0)
                {
                    UserBLL.Create(info);
                }
                else
                {
                    UserBLL.Edit(info);
                }
            }
            listInportResult.Add("导入完成！");
            return Json(new APIJson(1, "提交成功", listInportResult));


        }

        private string CheckDataTableHead(DataTable dt)
        {
            if (null == dt || dt.Rows.Count == 0)
            {
                return "数据内容为空";
            }
            List<string> DtColumn = GetColumnDetail();

            foreach (string item in DtColumn)
            {
                if (!dt.Columns.Contains(item))
                {
                    return string.Format("表格中没有找到列《{0}》,该行在数据导入中是必需的", item);
                }
            }

            return string.Empty;
        }

        private static List<string> GetColumnDetail()
        {
            List<string> DtColumn = new List<string>();
            DtColumn.Add("ID");
            DtColumn.Add("登录名");
            DtColumn.Add("真实姓名");
            DtColumn.Add("单位名称");
            DtColumn.Add("是否启用");
            //DtColumn.Add("微信OpenID");
            //DtColumn.Add("微信昵称");
            DtColumn.Add("创建时间");
            DtColumn.Add("角色");
            DtColumn.Add("学科");


            return DtColumn;
        }

        #endregion

        #region InportLight

        [AllowAnonymous]
        public ActionResult InportLight()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public ActionResult InportLightPost2()
        {
            bool HidenIsInport = Function.GetRequestString("HidenIsInport").ToLower() == "true";
            string DdlWorkSheet = Function.GetRequestString("DdlWorkSheet");
            string HidenFileURL = Function.GetRequestString("HidenFileURL");

            HidenFileURL = Server.MapPath(HidenFileURL);
            DataTable dt = new DataTable();

            try
            {
                dt = Tool.ExcelHelper.ExcelToDatatable(HidenFileURL, DdlWorkSheet);
            }
            catch (Exception ex)
            {
                return Json(new APIJson(-1, "系统出错:" + ex.Message));
            }
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(dt.Rows[i][0].ToString().Trim()))
                {
                    dt.Rows.RemoveAt(i);
                }
            }


            var ErrorMsg = CheckDataTableHeadLight(dt);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                return Json(new APIJson(-1, ErrorMsg));
            }
            if (!HidenIsInport)
            {
                return Json(new APIJson(0,"解析成功",dt.DataTableToList()));
            }
            var ListSubject = SubjectBLL.GetList(a => true).ToList();
            int DefaultRuleIDWhenInport = Function.ConverToInt(ProfilesBLL.GetValue(ProfilesInfo.InportSetting.DefaultRuleIDWhenInport));
            RuleInfo infoRule = RuleBLL.GetList(a => a.ID == DefaultRuleIDWhenInport).FirstOrDefault();


            List<string> listInportResult = new List<string>();
            List<UserInfo> listUserInport = new List<UserInfo>();
            foreach (DataRow item in dt.Rows)
            {

                UserInfo infoUser = new UserInfo();
                infoUser.DepartmentID = CurrentUser.DepartmentID;
                infoUser.Name= item["真实姓名"].ToString().Trim();
                infoUser.Code= item["登录名"].ToString().Trim();
                infoUser.PassWord= item["初始密码"].ToString().Trim();
                infoUser.Email = string.Empty;
                infoUser.Tel = string.Empty;
                infoUser.CreateDate = DateTime.Now;
                infoUser.LastDate = DicInfo.DateZone;
                infoUser.Enable = true;
                infoUser.LocationX =0;
                infoUser.LocationY = 0;
                infoUser.WechatOpenID = string.Empty;
                infoUser.WechatNameNick = string.Empty;
                infoUser.WechatHeadImg = string.Empty;
                infoUser.Sex = string.Empty;
                infoUser.IDCard = string.Empty;
                infoUser.TypeID = -3;
                infoUser.DefaultSubjectID =  0;
                infoUser.Email = string.Empty;
                var HelpText1 = item["学科"].ToString().Trim();

                var UserSubjectinfo = ListSubject.Where(a => HelpText1.Split('|').Contains(a.Name));
                if (UserSubjectinfo.Count()>0)
                {
                    infoUser.DefaultSubjectID = UserSubjectinfo.FirstOrDefault().ID;
                    infoUser.SubjectInfo = new List<SubjectInfo>();
                    foreach (var itemSubject in UserSubjectinfo)
                    {
                        infoUser.SubjectInfo.Add(itemSubject);
                    }
                }
                else
                {
                    return Json(new APIJson(-1, string.Format("提交失败，序号:{0}所在行中，对应学科数据无法识别", item["序号"].ToString())));
                }
                infoUser.RuleInfo.Add(infoRule);

                if (string.IsNullOrEmpty(infoUser.Name)||string.IsNullOrEmpty(infoUser.Code))
                {
                    return Json(new APIJson(-1, string.Format("提交失败，序号:{0}所在行中，用户名或登录名为空白，请先补填完整", item["序号"].ToString())));
                }
                UserInfo infoDb = UserBLL.GetList(a => a.Code == infoUser.Code || a.Name== infoUser.Name).FirstOrDefault();
                if (null!=infoDb)
                {
                    return Json(new APIJson(-1, string.Format("提交失败，序号:{0}所在行中，用户名或登录名被占用，请为更改【可以登录名后加上数字或字母】", item["序号"].ToString())));
                }
                if (infoUser.PassWord.Length < 6|| !ValidatePassWord(infoUser))
                {
                    return Json(new APIJson(-1, string.Format("提交失败，序号:{0}所在行中，密码不符合要求，必需包括数字和字母，并且长度在6位数以上", item["序号"].ToString())));
                }
                listUserInport.Add(infoUser);
            }
            foreach (var item in listUserInport)
            {
                UserBLL.Create(item);
            }
            listInportResult.Add("导入完成！");
            return Json(new APIJson(2, "提交成功，"+listUserInport.Count()+"个帐户成功导入"));


        }

        private string CheckDataTableHeadLight(DataTable dt)
        {
            if (null == dt || dt.Rows.Count == 0)
            {
                return "数据内容为空";
            }
            List<string> DtColumn = GetColumnDetailLight();

            foreach (string item in DtColumn)
            {
                if (!dt.Columns.Contains(item))
                {
                    return string.Format("表格中没有找到列《{0}》,该行在数据导入中是必需的", item);
                }
            }

            return string.Empty;
        }

        private static List<string> GetColumnDetailLight()
        {
            List<string> DtColumn = new List<string>();
            DtColumn.Add("序号");
            DtColumn.Add("登录名");
            DtColumn.Add("真实姓名");
            DtColumn.Add("初始密码");
            DtColumn.Add("学科");
            
            return DtColumn;
        }

        #endregion

    }
}