﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace WebTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string connStr = @"Server=(LocalDB)\v11.0; Integrated Security=true ;AttachDbFileName=" + Server.MapPath("~/App_Data/SqlSugar.mdf");
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                db.BeginTran();//开启事务，可以不使用事务
                //db.CommitTran using释放资源时自动执行

                db.Sqlable.IsNoLock = true; //查询是允许脏读的 

                try
                {
                    /*********************************************1、实体生成****************************************************/

                    //根据当前数据库生成所有表的实体类文件 （参数：SqlSugarClient ，文件目录，命名空间）
                    //db.ClassGenerating.CreateClassFiles(db,Server.MapPath("~/Models"),"Models");
                    //根据表名生成实体类文件
                    //db.ClassGenerating.CreateClassFilesByTableNames(db, Server.MapPath("~/Models"), "Models" , "student","school");

                    //根据表名生成class字符串
                    var str = db.ClassGenerating.TableNameToClass(db, "student");

                    //根据SQL语句生成class字符串
                    var str2 = db.ClassGenerating.SqlToClass(db, "select top 1 * from student", "student");



                    /*********************************************2、查询****************************************************/

                    //根据sql语句映射成实体
                    var School = db.SqlQuery<school>("select * from School");



                    //Queryable<T>查询扩展函数
                    var student = db.Queryable<Student>().Where(it => it.name == "张三").Where(c => c.id > 10).ToList();
                    var student2 = db.Queryable<Student>().Where(c => c.id > 10).Order("id").Skip(10).Take(20).ToList();//取10-20条
                    var student22 = db.Queryable<Student>().Where(c => c.id > 10).Order("id asc").ToPageList(2, 2);//分页
                    var student3 = db.Queryable<Student>().Where(c => c.id > 10).Order("id").Skip(2).ToList();//从第2条开始
                    var student4 = db.Queryable<Student>().Where(c => c.id > 10).Order("id").Take(2).ToList();//top2



                    //---------联表查询---------//

                    /*db.Sqlable是一个SQL语句生成帮助类*/
                    string sql = db.Sqlable.TableToSql("Student");//sql1等于 select * from Student
                    string sql1 = db.Sqlable.TableToSql<Student>();// sql1等于 select * from Student


                    //联表查询
                    string sql2 = db.Sqlable.MappingTable<Student, school>("t1.sch_id=t2.id?"/* 最后加?代表left join否则inner join  */).WhereAfter("sex='男' order by t2.id").SelectToSql("t1.*,t2.name as school_name");
                    var studentView = db.SqlQuery<Student_View>(sql2);

                    //联表分页查询
                    string pageSql = db.Sqlable.MappingTable<Student, school>("t1.sch_id=t2.id").SelectToPageSql(3,10,"t1.id asc","t1.*,t2.name as school_name");
                    var studentView2 = db.SqlQuery<Student_View>(pageSql);

            

                    /*********************************************3、添加****************************************************/

                    school s = new school()
                    {
                        name = "蓝翔"
                    };
                    var id = db.Insert(s);



                    /*********************************************4、修改****************************************************/

                    db.Update<school>(new { name = "蓝翔2" }, new { id = id });





                    /*********************************************5、删除****************************************************/

                    db.Delete<school>(id);





                    /*********************************************6、基类****************************************************/

                    db.ExecuteCommand(sql);
                    db.GetDataTable(sql);
                    db.GetString(sql);
                    db.GetInt(sql);
                    db.GetScalar(sql);
                    using (SqlDataReader read = db.GetReader(sql))
                    { 
                    
                    }

                }
                catch (Exception ex)
                {

                    db.RollbackTran();
                    throw ex;
                }

            }
            ;
            //var xx = SqlTool.CreateMappingTable(20);
            Console.Read();
        }



    }



}