﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSystem.Models
{
    /*
     * 表中文名称	    系统参数表				
     * 表英文名称	    SystemParameter				
     * 表用途	        系统中所有列表中的选项汇总集合				
     * 所在位置	        本地Access、FTP服务器Access和sql 服务器				
     * 主键	            ParameterType、ParameterNO				
     * 备注					
					
     * 中文名称	    英文字段	        类型	            是否可为空	外键关系	    备注
     * 参数类型	    ParameterType	nvarchar(50)	否		
     * 参数编号	    ParameterNO	    int	            否		
     * 参数值	    Value	        char(100)	    否		
     * 是否可修改	Revisable	    int	            否		                0:不可修改；1：可修改
     */
    public class SystemParameterModel : TableModel
    {

        /// <summary>
        /// 参数类型
        /// </summary>
        public String ParameterType { get; set; }

        /// <summary>
        /// 参数编号
        /// </summary>
        public int ParameterNO { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// 是否可修改
        /// 0:不可修改；1：可修改
        /// </summary>
        public int Revisable { get; set; }


        public SystemParameterModel()
        {
            TableName = "SystemParameter";
        }

        public override string getAllRecordSQL()
        {
            return @"SELECT * FROM " + TableName;
        }

        /// <summary>
        /// 根据ParameterType和ParameterNO
        /// </summary>
        /// <returns></returns>
        public override string getMyRecordSQL()
        {
            //SELECT 列名称 FROM 表名称
            return String.Format(@"SELECT ParameterType, ParameterNO, Value, Revisable FROM {0} WHERE ParameterType = '{1}', ParameterNO = '{2}'", TableName, ParameterType, ParameterNO );
        }
        
        /// <summary>
        /// ParameterType 是主键
        /// </summary>
        /// <returns></returns>
        public override string getRecordByKeySQL()
        {
            return String.Format(@"SELECT ParameterType, ParameterNO, Value, Revisable FROM {0} WHERE ParameterType = '{1}'", TableName, ParameterType);
        }


    }
}