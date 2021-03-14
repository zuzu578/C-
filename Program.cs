using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationSync
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.GetCurrentClassLogger().Info($"휴가 동기화 시작");
            SyncVacation();
            LogManager.GetCurrentClassLogger().Info($"휴가 동기화 종료{Environment.NewLine}");

            LogManager.GetCurrentClassLogger().Info($"출장 동기화 시작");
            SyncBusiness();
            LogManager.GetCurrentClassLogger().Info($"출장 동기화 종료{Environment.NewLine}");
        }

        static void SyncVacation()
        {
            List<string> create_list = new List<string>();
            List<string> update_list = new List<string>();

            string sql = string.Empty;

            try
            {
                #region Step 1 : 휴가 정보 조회
                //oracle 에있는 휴가 정보를 조회하겠죠
                //oracle view table (select 만 할수있고 수정X)
                //고객사 --> view table --> 개발자 --> 개발자의 역할 : 고객이 준 table을 수정 X select 해서 --> 솔루션에 data 넣는역할(연동)
                //연동 : 고객이 프로그램을 사용할수있게 끔 만들어주는 작업
                // 내생각이 그래요


                DBHelper dbHelper = new DBHelper();
                DBHelper_Oracle dbHeleper_Oracle = new DBHelper_Oracle();

                string connectionString = ConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
                string connectionString_Oracle = ConfigurationManager.ConnectionStrings["ConnDB_HR"].ConnectionString;

                int sync_count = 0;
                //sql --> VW_PCOFF_VACATION (view table) select
                //select => 고객사의 요구조건에 따라서 달라지는겁니다. 

                sql = @"SELECT USER_ID, VACATION_NAME, VACATION_TYPE, VACATION_DAYS, START_DATE, END_DATE, START_TIME, END_TIME, MEMO, WORK_HOUR, REWARD_YN, ALLDAY_YN
                        FROM VW_PCOFF_VACATION";

                #endregion

                using (DataTable dt = dbHeleper_Oracle.ExecuteSQL(sql, connectionString_Oracle))
                {
                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        LogManager.GetCurrentClassLogger().Info($"HR DB 데이터 없음");
                        return;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> item = new Dictionary<string, object>();
                        string user_id = dr["USER_ID"].ToString();
                        string vacation_name = dr["VACATION_NAME"].ToString();
                        string vacation_type = dr["VACATION_TYPE"].ToString();
                        string vacation_days = dr["VACATION_DAYS"].ToString();
                        string start_date = DateTime.Parse(dr["START_DATE"].ToString()).ToString("yyyy-MM-dd");
                        string end_date = DateTime.Parse(dr["END_DATE"].ToString()).ToString("yyyy-MM-dd");
                        string start_time = dr["START_TIME"].ToString();
                        string end_time = dr["END_TIME"].ToString();
                        string memo = dr["MEMO"].ToString() == "" ? "휴가신청" : dr["MEMO"].ToString();
                        string work_hour = dr["WORK_HOUR"].ToString();
                        string reward_YN = dr["REWARD_YN"].ToString();
                        string allday_YN = dr["ALLDAY_YN"].ToString();
                        string approval_start_time = start_time + ".000";
                        string approval_end_time = end_time + ".000";

                        if (allday_YN == "Y")
                        {
                            start_time = "00:00:00.0000000";
                            end_time = "23:59:59.0000000";

                            approval_start_time = "00:00:00.000";
                            approval_end_time = "23:59:59.000";
                        }

                        sql = $@"DECLARE @vacation_days FLOAT
                                 DECLARE @vacation_days_minu FLOAT
        
                                 DECLARE @vacation_type_no  INT
                                 DECLARE @approval_no INT
                                 DECLARE @user_no INT
                            
                                 SELECT @user_no = U.user_no
                                 FROM [dbo].[tb_user] U WITH(NOLOCK)
                                 WHERE U.user_id = '{user_id}'

                                 SELECT @vacation_days = V.vacation_days, @vacation_days_minu = -V.vacation_days
                                 FROM [dbo].[tb_vacation_type] V
                                 WHERE V.type = '{vacation_type}' AND V.vacation_name = '{vacation_name}' AND V.daily_work_hour = '{work_hour}'

                                 SELECT @vacation_type_no = V.vacation_type_no
                                 FROM [dbo].[tb_vacation_type] V
                                 WHERE V.type = '{vacation_type}' AND V.vacation_name = '{vacation_name}' AND V.daily_work_hour = '{work_hour}'

                                 IF(@vacation_type_no IS NULL)
                                 BEGIN
                                    INSERT INTO [dbo].[tb_vacation_type] WITH(ROWLOCK)
                                    (company_no, vacation_name, type, daily_work_hour, vacation_days, apply_YN, use_YN, allday_YN, start_time, end_time)
                                    VALUES
                                    (1, '{vacation_name}', '{vacation_type}', '{work_hour}', '{vacation_days}', '{reward_YN}', 'Y', '{allday_YN}', '{start_time}', '{end_time}')

                                    SET @vacation_type_no = SCOPE_IDENTITY()
                                    SET @vacation_days_minu = -{vacation_days}
                                    SET @vacation_days = {vacation_days}                                 
                                 END
                                 
                                 IF(@user_no IS NOT NULL AND @vacation_type_no IS NOT NULL)
                                 BEGIN
                                    IF NOT EXISTS(
                                        SELECT *
                                        FROM [dbo].[tb_vacation] V WITH(NOLOCK)
                                        INNER JOIN [dbo].[tb_approval] A ON A.approval_no = V.approval_no AND A.status = 'accept'
                                        INNER JOIN [dbo].[tb_user] U ON U.user_no = V.user_no AND U.user_id = '{user_id}' AND U.retireYN = 'N'
                                        WHERE CONVERT(DATE, V.start_date) = '{start_date}'
                                    )
                                    BEGIN
                                        INSERT INTO [dbo].[tb_approval] WITH(ROWLOCK)
                                        (user_no, approval_type, start_time, end_time, accept_time, status, memo, accept_user_no, vacation_type_no, vacation_name, vacation_days, vacation_work_hour)
                                        VALUES
                                        (@user_no, 'vacation', '{start_date + " " + approval_start_time}', '{end_date + " " + approval_end_time}', null, 'accept', '{memo}', 1, @vacation_type_no, '{vacation_name}', @vacation_days_minu, '{work_hour}')

                                        SET @approval_no = SCOPE_IDENTITY()

                                         INSERT INTO [dbo].[tb_vacation] WITH(ROWLOCK)
                                         (user_no, vacation_type_no, vacation_name, vacation_days, start_date, end_date, memo, approval_no, work_hour, reward_YN, allday_YN, start_time, end_time)
                                         VALUES
                                         (@user_no, @vacation_type_no, '{vacation_name}', @vacation_days_minu, '{start_date + " 00:00:00.000"}', '{end_date + " 23:59:59.000"}', '{memo}', @approval_no, '{work_hour}', '{reward_YN}', '{allday_YN}', '{start_time}', '{end_time}')

                                         INSERT INTO [dbo].[tb_approval_line] WITH(ROWLOCK)
                                         (approval_no, approval_order, user_sent_to, approvalYN, approval_time)
                                         VALUES
                                         (@approval_no, 1, 1, 'Y', SYSDATETIME())

                                         -- 휴가 히스토리
                                         INSERT INTO [dbo].[tb_vacation_history] WITH(ROWLOCK)
                                         (user_no, approval_no, vacation_name, vacation_days)
                                         SELECT U.user_no, A.approval_no, A.vacation_name, A.vacation_days
                                         FROM [dbo].[tb_user] U
                                         INNER JOIN [dbo].[tb_approval] A ON U.user_no = A.user_no
                                         WHERE A.approval_no = @approval_no AND A.approval_type = 'vacation' AND A.status = 'accept'

                                         SELECT @approval_no AS approval_no
                                    END
                                    ELSE
                                    BEGIN
                                        SELECT @approval_no = A.approval_no
                                        FROM [dbo].[tb_vacation] V WITH(NOLOCK)
                                        INNER JOIN [dbo].[tb_approval] A ON A.approval_no = V.approval_no AND A.status = 'accept'
                                        INNER JOIN [dbo].[tb_user] U ON U.user_no = V.user_no AND U.user_id = '{user_id}' AND U.retireYN = 'N'
                                        WHERE CONVERT(DATE, V.start_date) = '{start_date}'

                                        UPDATE A
                                        SET A.memo = '{memo}', A.start_time = '{start_date + " " + approval_start_time}', A.end_time = '{end_date + " " + approval_end_time}',
                                            A.vacation_name = '{vacation_name}', A.vacation_days = @vacation_days_minu, A.vacation_work_hour = '{work_hour}'
                                        FROM [dbo].[tb_approval] A WITH(ROWLOCK)
                                        WHERE A.approval_no = @approval_no

                                        UPDATE V
                                        SET V.vacation_type_no = @vacation_type_no, V.vacation_name = '{vacation_name}', V.vacation_days = @vacation_days_minu, memo = '{memo}', work_hour = '{work_hour}',
                                            V.reward_YN = '{reward_YN}', V.allday_YN = '{allday_YN}', start_time = '{start_time}', end_time = '{end_time}'
                                        FROM [dbo].[tb_vacation] V WITH(ROWLOCK)
                                        WHERE V.approval_no = @approval_no

                                        SELECT @approval_no AS approval_no
                                    END
                                 END";

                        using (DataTable dt_vacation = dbHelper.ExecuteSQL(sql, connectionString))
                        {
                            if (dt_vacation != null || dt_vacation.Rows.Count > 0)
                            {
                                string approval_no = dt_vacation.Rows[0]["approval_no"].ToString();
                                create_list.Add(approval_no);
                                LogManager.GetCurrentClassLogger().Info($"approval_no : {approval_no}");
                                sync_count++;
                            }
                        }
                    }

                    LogManager.GetCurrentClassLogger().Info($"총 휴가 추가 및 수정 : {sync_count}");

                    #region Step 3 : TimeKeeper  휴가 정보 조회

                    List<string> vacation_list = new List<string>();

                    sql = @"SELECT A.approval_no
                            FROM [dbo].[tb_approval] A WITH(NOLOCK)
                            INNER JOIN [dbo].[tb_vacation] V ON V.approval_no = A.approval_no AND A.status = 'accept'
                            WHERE CONVERT(DATE,A.end_time) > CONVERT(DATE, DATEADD(DAY, -1, SYSDATETIME())) ";

                    using (DataTable dt_vacation = dbHelper.ExecuteSQL(sql, connectionString))
                    {
                        if (dt_vacation == null || dt_vacation.Rows.Count <= 0)
                        {
                            LogManager.GetCurrentClassLogger().Info($"휴가 데이터 없음");
                            return;
                        }

                        foreach (DataRow dr in dt_vacation.Rows)
                        {
                            string approval_no = Convert.ToString(dr["approval_no"]);
                            vacation_list.Add(approval_no);
                        }
                    }

                    #endregion

                    #region Step 4 : TimeKeeper 휴가 삭제

                    sync_count = 0;

                    foreach (string approval_no in vacation_list.Except(create_list))
                    {
                        sql = $@"DECLARE @user_no INT
                                
                                 SELECT @user_no = U.user_no
                                 FROM [dbo].[tb_vacation] V WITH(NOLOCK)
                                 INNER JOIN [dbo].[tb_approval] A ON A.approval_no = V.approval_no AND A.status = 'accept'
                                 INNER JOIN [dbo].[tb_user] U ON U.user_no = V.user_no AND U.retireYN = 'N'
                                 WHERE V.approval_no = '{approval_no}'

                                 UPDATE A
                                 SET A.status = 'refuse'
                                 FROM [dbo].[tb_approval] A WITH(ROWLOCK)
                                 WHERE A.approval_no = {approval_no}

                                 INSERT INTO [dbo].[tb_vacation_history] WITH(ROWLOCK)
                                 (user_no, vacation_name, approval_no, vacation_days)
                                 VALUES
                                 (@user_no, '[취소]', {approval_no}, -(SELECT vacation_days FROM tb_vacation WHERE approval_no = {approval_no}))

                                 DELETE FROM [dbo].[tb_vacation] WITH(ROWLOCK)
                                 WHERE user_no = @user_no AND approval_no = {approval_no}

                                 SELECT {approval_no} AS approval_no";

                        dbHelper.ExecuteSQL(sql, connectionString);
                        ++sync_count;
                        LogManager.GetCurrentClassLogger().Info($"휴가 삭제 : {approval_no}");
                    }

                    LogManager.GetCurrentClassLogger().Info($"총 휴가 삭제 : {sync_count}");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Info(sql);
                LogManager.GetCurrentClassLogger().Info($"StackTrace : {ex.StackTrace}");
                LogManager.GetCurrentClassLogger().Info($"Message : {ex.Message}");
            }
        }

        static void SyncBusiness()
        {
            List<string> create_list = new List<string>();

            string sql = string.Empty;

            try
            {
                DBHelper dbHelper = new DBHelper();
                DBHelper_Oracle dbHeleper_Oracle = new DBHelper_Oracle();

                string connectionString = ConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
                string connectionString_Oracle = ConfigurationManager.ConnectionStrings["ConnDB_HR"].ConnectionString;

                sql = @"SELECT USER_ID, BUSINESS_TRIP_TYPE AS BUSINESS_TRIP_NAME, BUSINESS_TRIP_TYPE, BUSINESS_TRIP_DAYS, START_DATE, END_DATE, DAILY_WORK_HOUR, DAILY_OVER_HOUR, MEMO
                        FROM VW_PCOFF_BUSINESS_TRIP
                        WHERE CONVERT(DATE, START_DATE) > CONVERT(DATE, DATEADD(DAY, -1, SYSDATETIME()))";

                int sync_count = 0;

                using (DataTable dt = dbHeleper_Oracle.ExecuteSQL(sql, connectionString_Oracle))
                {
                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        LogManager.GetCurrentClassLogger().Info($"HR DB 데이터 없음");
                        return;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> item = new Dictionary<string, object>();

                        string user_id = dr["USER_ID"].ToString();
                        string business_trip_name = dr["BUSINESS_TRIP_NAME"].ToString();
                        string business_trip_type = dr["BUSINESS_TRIP_TYPE"].ToString();
                        string business_trip_days = dr["BUSINESS_TRIP_DAYS"].ToString();
                        string start_date = DateTime.Parse(dr["START_DATE"].ToString()).ToString("yyyy-MM-dd 00:00:00.000");
                        string end_date = DateTime.Parse(dr["END_DATE"].ToString()).ToString("yyyy-MM-dd 23:59:59.000");
                        string daily_work_hour = dr["DAILY_WORK_HOUR"].ToString();
                        string daily_over_hour = dr["DAILY_OVER_HOUR"].ToString();
                        string memo = dr["MEMO"].ToString() == "" ? "출장신청" : dr["MEMO"].ToString();

                        sql = $@"DECLARE @user_no INT
                                 DECLARE @business_trip_type_no INT
                                 DECLARE @approval_no INT

                                 SELECT @user_no = U.user_no
                                 FROM [dbo].[tb_user] U WITH(NOLOCK)
                                 WHERE U.user_id = '{user_id}'

                                 SELECT @business_trip_type_no = B.business_trip_type_no
                                 FROM [dbo].[tb_business_trip_type] B
                                 WHERE B.type_name = '{business_trip_type}' AND B.business_trip_name = '{business_trip_name}' AND B.daily_work_hour = '{daily_work_hour}' AND B.daily_over_hour = '{daily_over_hour}'

                                 IF(@business_trip_type_no IS NULL)
                                 BEGIN
                                    INSERT INTO [dbo].[tb_business_trip_type] WITH(ROWLOCK)
                                    (company_no, type_name, business_trip_name, daily_work_hour, daily_over_hour, use_YN, delete_YN)
                                    VALUES
                                    (1, '{business_trip_type}', '{business_trip_name}', '{daily_work_hour}', '{daily_over_hour}', 'Y', 'Y')

                                    SET @business_trip_type_no = SCOPE_IDENTITY()
                                 END

                                 IF(@user_no IS NOT NULL AND @business_trip_type_no IS NOT NULL)
                                 BEGIN
                                    IF NOT EXISTS(
                                        SELECT *
                                        FROM [dbo].[tb_business_trip] B WITH(NOLOCK)
                                        INNER JOIN [dbo].[tb_approval] A ON A.approval_no = B.approval_no AND A.status = 'accept'
                                        INNER JOIN [dbo].[tb_user] U ON U.user_no = B.user_no AND U.user_id = '{user_id}' AND retireYN = 'N'
                                        WHERE B.start_date = '{start_date}' AND B.end_date = '{end_date}'
                                    )
                                    BEGIN
                                        INSERT INTO [dbo].[tb_approval] WITH(ROWLOCK)
                                        (user_no, approval_type, start_time, end_time, accept_time, status, memo, accept_user_no, business_trip_type_no, business_trip_name, business_trip_daily_work_hour, business_trip_daily_over_hour)
                                        VALUES
                                        (@user_no, 'business_trip', '{start_date}', '{end_date}', null, 'accept', '{memo}', 1, @business_trip_type_no, '{business_trip_name}', '{daily_work_hour}', '{daily_over_hour}')

                                        SET @approval_no = SCOPE_IDENTITY()

                                        INSERT INTO [dbo].[tb_approval_line] WITH(ROWLOCK)
                                        (approval_no, approval_order, user_sent_to, approvalYN, approval_time)
                                        VALUES
                                        (@approval_no, 1, 1, 'Y', SYSDATETIME())
                                        
                                        INSERT INTO [dbo].[tb_business_trip] WITH(ROWLOCK)
                                        (user_no, business_trip_type_no, total_business_trip_period_days, start_date, end_date, daily_work_hour, daily_over_hour, memo, approval_no)
                                        VALUES
                                        (@user_no, @business_trip_type_no, 1, '{start_date}', '{end_date}', '{daily_work_hour}', '{daily_over_hour}', '{memo}', @approval_no)

                                        -- 출장 히스토리
                                        INSERT INTO [dbo].[tb_business_trip_history] WITH(ROWLOCK)
                                        (user_no, business_trip_name, approval_no, business_trip_days)
                                        VALUES
                                        (@user_no, '{business_trip_name}', @approval_no, 1)

                                        SELECT @approval_no AS approval_no
                                    END
                                    ELSE
                                    BEGIN
                                        SELECT @approval_no = A.approval_no, @user_no = U.user_no
                                        FROM [dbo].[tb_business_trip] B WITH(NOLOCK)
                                        INNER JOIN [dbo].[tb_approval] A ON A.approval_no = B.approval_no AND A.status = 'accept'
                                        INNER JOIN [dbo].[tb_user] U ON U.user_no = B.user_no AND U.user_id = '{user_id}' AND U.retireYN = 'N'
                                        WHERE B.start_date = '{start_date}' AND B.end_date = '{end_date}'        

                                        UPDATE A
                                        SET A.memo = '{memo}', A.business_trip_type_no = @business_trip_type_no, A.business_trip_name = '{business_trip_name}',
                                            A.business_trip_daily_work_hour = '{daily_work_hour}', A.business_trip_daily_over_hour = '{daily_over_hour}'
                                        FROM [dbo].[tb_approval] A WITH(ROWLOCK)
                                        WHERE A.approval_no = @approval_no
                                            
                                        UPDATE B
                                        SET B.business_trip_type_no = @business_trip_type_no, B.daily_work_hour = '{daily_work_hour}', B.daily_over_hour = '{daily_over_hour}',
                                            B.memo = '{memo}'
                                        FROM [dbo].[tb_business_trip] B WITH(ROWLOCK)
                                        WHERE B.approval_no = @approval_no         

                                        SELECT @approval_no AS approval_no
                                    END
                                 END";

                        using (DataTable dt_business = dbHelper.ExecuteSQL(sql, connectionString))
                        {
                            if (dt_business != null && dt_business.Rows.Count > 0)
                            {
                                string approval_no = dt_business.Rows[0]["approval_no"].ToString();
                                create_list.Add(approval_no);
                                LogManager.GetCurrentClassLogger().Info($"approval_no : {approval_no}");
                                sync_count++;
                            }
                        }
                    }

                    LogManager.GetCurrentClassLogger().Info($"총 출장 추가 및 수정 : {sync_count}");

                    #region Step 3 : TimeKeeper 출장 정보 조회

                    List<string> business_list = new List<string>();

                    sql = @"SELECT A.approval_no
                            FROM [dbo].[tb_approval] A WITH(NOLOCK)
                            INNER JOIN [dbo].[tb_business_trip] B ON B.approval_no = A.approval_no AND A.status = 'accept'
                            INNER JOIN [dbo].[tb_user] U ON U.user_no = B.user_no AND U.retireYN = 'N'
                            WHERE CONVERT(DATE,A.end_time) > CONVERT(DATE, DATEADD(DAY, -1, SYSDATETIME()))";

                    using (DataTable dt_business = dbHelper.ExecuteSQL(sql, connectionString))
                    {
                        if (dt_business == null || dt_business.Rows.Count <= 0)
                        {
                            LogManager.GetCurrentClassLogger().Info($"출장 데이터 없음");
                            return;
                        }

                        foreach (DataRow dr in dt_business.Rows)
                        {
                            string approval_no = Convert.ToString(dr["approval_no"]);
                            business_list.Add(approval_no);
                        }
                    }

                    #endregion

                    #region Step 4 : TimeKeeper 출장 삭제

                    sync_count = 0;

                    foreach (string approval_no in business_list.Except(create_list))
                    {
                        sql = $@"DECLARE @user_no INT
                                 DECLARE @approval_no INT                                 

                                 IF EXISTS(
                                           SELECT *
                                           FROM [dbo].[tb_business_trip] B WITH(NOLOCK)
                                           INNER JOIN [dbo].[tb_approval] A ON A.approval_no = B.approval_no AND A.status = 'accept'
                                           INNER JOIN [dbo].[tb_user] U ON U.user_no = B.user_no AND U.retireYN = 'N'
                                           WHERE B.approval_no = '{approval_no}'
                                  )
                                  BEGIN
                                     SELECT @approval_no = {approval_no}, @user_no = U.user_no
                                     FROM [dbo].[tb_business_trip] B WITH(NOLOCK)
                                     INNER JOIN [dbo].[tb_approval] A ON A.approval_no = B.approval_no AND A.status = 'accept'
                                     INNER JOIN [dbo].[tb_user] U ON U.user_no = B.user_no AND U.retireYN = 'N'
                                     WHERE B.approval_no = '{approval_no}'

                                     UPDATE A
                                     SET A.status = 'refuse'
                                     FROM [dbo].[tb_approval] A WITH(ROWLOCK)
                                     WHERE A.approval_no = @approval_no
                    
                                     INSERT INTO [dbo].[tb_business_trip_history] WITH(ROWLOCK)
                                     (user_no, business_trip_name, approval_no, business_trip_days)
                                     VALUES
                                     (@user_no, '[취소]', @approval_no, 1)

                                     DELETE FROM [dbo].[tb_business_trip] WITH(ROWLOCK)
                                     WHERE user_no = @user_no AND approval_no = @approval_no
                
                                     SELECT @approval_no AS approval_no
                                  END";

                        dbHelper.ExecuteSQL(sql, connectionString);
                        ++sync_count;
                        LogManager.GetCurrentClassLogger().Info($"출장 삭제 : {approval_no}");
                    }

                    LogManager.GetCurrentClassLogger().Info($"총 출장 삭제 : {sync_count}");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Info(sql);
                LogManager.GetCurrentClassLogger().Info($"StackTrace : {ex.StackTrace}");
                LogManager.GetCurrentClassLogger().Info($"Message : {ex.Message}");
            }
        }


    }
}
