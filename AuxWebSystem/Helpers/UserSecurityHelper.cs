﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Globalization;

using AuxWebSystem.Models;

namespace AuxWebSystem.Helpers
{
    public class UserSecurityHelper
    {
        /// <summary>
        /// 储存Session的字段
        /// </summary>
        public static readonly String sessionName = @"UserInfo";

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model">登录Model</param>
        /// <returns>能否登录成功</returns>
        public static void Login(LoginModel model)
        {
            //TODO: add login check code here
            if (DataBaseHelper.hasMyRecord(model))
            {
                HttpSessionState session = HttpContext.Current.Session;
                UserTableModel usermodel = new UserTableModel();
                usermodel.LogName = model.LogName;
                usermodel.Password = model.Password;
                DataBaseHelper.Update(model);
                DataBaseHelper.fillOneRecordToModel(usermodel);
                session[sessionName] = usermodel;
            }
            else
            {
                throw new UserSecurityException("用户名或密码不对");
            }
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        public static void Logout()
        {
            HttpSessionState session = HttpContext.Current.Session;
            UserTableModel simModel = (UserTableModel)session[sessionName];
            session.Remove(sessionName);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model">注册信息</param>
        /// <returns></returns>
        public static void Register(UserTableModel model)
        {
            //TODO: can do better
            UserSecurityException exce = new UserSecurityException();
            bool hasErr = false;
            if (null == model.LogName || "".Equals(model.LogName))
            {
                hasErr = true;
                exce.Error.LogName = "登录名为空";
            }

            if (null == model.RealName || "".Equals(model.RealName) )
            {
                hasErr = true;
                exce.Error.RealName = "真实姓名为空";
            }

            if (null != model.Email && !"".Equals(model.Email) && !model.checkEmail())
            {
                hasErr = true;
                exce.Error.Email = "Email不符合规范";
            }

            if (null != model.CellPhone && !"".Equals(model.CellPhone) && !model.checkCellPhone())
            {
                hasErr = true;
                exce.Error.CellPhone = "手机号码不符合规范";
            }



            var table = HttpRuntime.Cache[UserTableModel.CacheName] as DataTable;
            
            var row = table.Select(String.Format(" {0} = '{1}' ", "LogName", model.LogName ));
            if (row.Length >= 1)
            {
                hasErr = true;
                exce.Error.LogName = "登录名重复";
            }

            row = table.Select(String.Format(" {0} = '{1}' ", "RealName", model.RealName));

            if (row.Length >= 1)
            {
                hasErr = true;
                exce.Error.RealName = "真实姓名重复";
            }

            if (hasErr)
            {
                throw exce;
            }

            if (!DataBaseHelper.Insert(model))
            {
                throw new UserSecurityException("发生未知错误");
            }

        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userID">用户名</param>
        public static void DeleteAccount(UserTableModel model)
        {
            //TODO: add delete account code
            if (!DataBaseHelper.Delete(model))
            {
                throw new UserSecurityException("删除失败");
            }
        }

        /// <summary>
        /// 更改用户信息
        /// </summary>
        /// <param name="model">更改的信息</param>
        /// <param name="orign">原来的信息</param>
        public static void ModifyUser(ModifyUserTableModel model, UserTableModel orign)
        {
            UserSecurityException exec = new UserSecurityException();
            if (null == model.Password || !model.Password.Equals(orign.Password))
            {
                exec.Error.Password = "原始密码不正确";
                throw exec;
            }

            bool haserr = false;
            if (null == model.NewPassword )
            {
                exec.Error.NewPassword = "请检查新密码";
                haserr = true;
            }

            if (null == model.ConfirmPassword || !model.ConfirmPassword.Equals(model.NewPassword))
            {
                exec.Error.ConfirmPassword = "密码输入不一致";
                haserr = true;
            }

            if (null != model.CellPhone && !"".Equals(model.CellPhone) && !model.checkCellPhone())
            {
                exec.Error.CellPhone = "请检查手机号码";
                haserr = true;
            }

            if (null != model.Email && !"".Equals(model.Email) && !model.checkEmail())
            {
                exec.Error.Email = "请检查邮箱";
                haserr = true;
            }

            if (haserr)
            {
                throw exec;
            }

            if (!DataBaseHelper.Update(model))
            {
                throw new UserSecurityException("修改失败，请检查网络，稍后再试");
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="UserID">用户ID</param>
        public static void ResetPassword(int UserID)
        {
            ModifyUserTableModel muserTable = new ModifyUserTableModel();
            muserTable.UserID = UserID;
            muserTable.NewPassword = "123456";
            if (!DataBaseHelper.Update(muserTable))
            {
                throw new UserSecurityException("重置密码失败");
            }
        }
    }

    /// <summary>
    /// 用户登录异常
    /// </summary>
    class UserSecurityException : Exception
    {
        public UserSecurityException(String message)
            : base(message)
        {
            Error = new ModifyUserTableModel();
        }
        public UserSecurityException()
            : base("")
        {
            Error = new ModifyUserTableModel();
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public ModifyUserTableModel Error { get; set; }
    }
}