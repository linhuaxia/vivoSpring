using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Vivo.Model;
using Tool;


namespace Vivo.web.Controllers
{
    public class PrizeResultController : BaseController
    {
        public static object objlocker = new object();
        public ActionResult C(PrizeResultInfo info)
        {
            if (string.IsNullOrEmpty(info.OpenID))
            {
                return Json(new APIJson(-1, "parms error ,can't find openid"));
            }
            if (string.IsNullOrEmpty(info.Name) || info.Name.Length > 50)
            {
                return Json(new APIJson(-1, "用户名不能为空，请输入重试!"));
            }
            if (!IsMobilePhone(info.Tel))
            {
                return Json(new APIJson(-1, "手机号输入不正确，请重新输入!"));
            }

            VivoAPIInfo infoAPIresult = GetAgentAndMarketLevel(info.SnNumber);

            if (infoAPIresult.Status == 0)
            {
                return Json(new APIJson(-1, infoAPIresult.message));
            }
            var infoPrizeResult = PrizeResultBLL.GetList(a => a.SnNumber == info.SnNumber).FirstOrDefault();
            if (null != infoPrizeResult)
            {
                string ExistMsg = "这个SN码已经参与过抽奖。";
                if (string.IsNullOrEmpty(infoPrizeResult.Result))
                {
                    ExistMsg += "谢谢参与，祝您好运";
                }

                return Json(new APIJson(-1, ExistMsg, infoPrizeResult.Result));
            }
            DateTime BuyTime = GetSnretailTime(info.SnNumber);
            int EnablePrizeHour = Function.ConverToInt(ProfilesBLL.GetValue("手机购买时间有效小时数"));
            if ((DateTime.Now - BuyTime).TotalHours > EnablePrizeHour)
            {
                return Json(new APIJson(-189, "抱歉，本次活动限定在购买日" + EnablePrizeHour + "小时内参与抽奖，您已超过限定时段"));
            }
            string ListEnableAgenterName = ProfilesBLL.GetValue("EnableAgenterName");
            if (!ListEnableAgenterName.Contains(infoAPIresult.AgenterName))
            {
                return Json(new APIJson(-123, "不在活动范围,无法抽奖。"));
            }
            var listPlan = PlanBLL.GetList(a => DbFunctions.DiffMinutes(a.DateBegin, DateTime.Now) >= 0
                                            && DbFunctions.DiffMinutes(a.DateEnd, DateTime.Now) <= 0
                                            && a.Enable).OrderByDescending(a=>a.SortID);
            if (listPlan.Count() == 0)
            {
                return Json(new APIJson(-123, "不在活动期间,无法抽奖。"));
            }
            info.SnApiResult = Newtonsoft.Json.JsonConvert.SerializeObject(infoAPIresult);
            info.CreateDate = DateTime.Now;


            bool ResultCreate = GetPrize(listPlan, infoAPIresult, info);
            if (!ResultCreate)
            {
                return Json(new APIJson(-1, "暂时无法进行抽奖，请重试"));
            }
            var Result = new
            {
                info.ID,
                info.OpenID,
                info.Tel,
                info.Result
            };
            return Json(new APIJson(0, "", Result));
        }

        private static Random rand = new Random();
        private bool GetPrize(IQueryable<PlanInfo> listPlan, VivoAPIInfo infoAPIresult, PrizeResultInfo info)
        {

            int randResult = rand.Next(0, 99);
            int AiQiYiRate = Tool.Function.ConverToInt(ProfilesBLL.GetValue("AiQiYiRate"), 50);

            info.PlanID = listPlan.FirstOrDefault().ID;
            if (infoAPIresult.MarketLevel == "乡镇")
            {
                lock (objlocker)
                {
                    info.Result = "";
                    return PrizeResultBLL.Create(info).ID > 0;
                }
            }



            lock (objlocker)
            {
                var infoPlanTarget = listPlan.FirstOrDefault(a => a.AgenterName == infoAPIresult.AgenterName && (a.MarketLevel.Contains(infoAPIresult.MarketLevel)));
                if (null != infoPlanTarget)
                {
                    if (infoPlanTarget.Mount - 1 <= infoPlanTarget.PrizeResultInfo.Count() && !infoPlanTarget.PrizeResultInfo.Any(a => a.Result ==infoPlanTarget.TypeFlag.ToString()))
                    {
                        info.PlanID = infoPlanTarget.ID;
                        info.Result =infoPlanTarget.TypeFlag.ToString();
                        return PrizeResultBLL.Create(info).ID > 0;
                    }
                }
                if (randResult <= AiQiYiRate)
                {
                    AiQiYiInfo infoEmpty = AiQiYiBLL.GetList(a => a.Status == 1).FirstOrDefault();
                    if (null != infoEmpty)
                    {
                        infoEmpty.Status = 0;
                        infoEmpty.OwnerUserOpenID = info.OpenID;
                        info.Result = infoEmpty.CodeNo;
                        if (null!=infoPlanTarget)
                        {
                            info.PlanID = infoPlanTarget.ID;
                        }
                        AiQiYiBLL.Edit(infoEmpty);
                        return PrizeResultBLL.Create(info).ID > 0;
                    }
                }
                info.Result = string.Empty;
                return PrizeResultBLL.Create(info).ID > 0;
            }


        }


        private const string APIURL = "http://sdapi.vivo.xyz:8888/api/Account/GetAgentAndMarketLevel?snnumber=";
        private VivoAPIInfo GetAgentAndMarketLevel(string snnumber)
        {
            try
            {
                string Result = DataHelper.GetHttpData(APIURL + snnumber);
                Result = Result.Trim('\"').Replace("\\", "");
                var info = Newtonsoft.Json.JsonConvert.DeserializeObject<VivoAPIInfo>(Result);
                return info;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private DateTime GetSnretailTime(string snnumber)
        {
            string URL = "http://sdapi.vivo.xyz:8888/api/Account/GetSnretailTime?snnumber=" + snnumber;
            try
            {
                string Result = DataHelper.GetHttpData(URL);
                Result = Result.Trim('\"').Replace("\\", "");
                return Tool.Function.ConverToDateTime(Result);
            }
            catch (Exception)
            {
                return DicInfo.DateZone;
            }
        }

        /// <summary>
        /// 判断输入的字符串是否是一个合法的手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^1[345678]\\d{9}$");
            return regex.IsMatch(input);

        }

    }
}