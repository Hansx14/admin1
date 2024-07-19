using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Attendance.Controllers
{
    public class HomeController : Controller
    {
        string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\riche\source\repos\Attendance\Attendance\App_Data\checker_attendance.mdf;Integrated Security=True";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Administration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Administration(FormCollection collection, HttpPostedFileBase img)
        {
            if (img == null || img.ContentLength <= 0)
            {
                Response.Write("<script>alert('Please upload an image.')</script>");
                return View();
            }
            string imag = Path.GetFileName(img.FileName);
                string logpath = "c:\\Upload";  // Make sure this directory exists on your server
                string filepath = Path.Combine(logpath, imag);
                img.SaveAs(filepath);

                var id = collection["adminId"];
                var lastname = collection["lastname"];
                var firstname = collection["firstname"]; // Corrected to match your SQL query
                var middle = collection["middle"];
                var mobile = collection["mobile"];
                var password = collection["password"];

                using (var db = new SqlConnection(connStr))
                {
                    db.Open();
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO ADMIN (ID, LASTNAME, FIRSTNAME, MIDDLE, MOBILE, PASSWORD, IMAGE) " +
                                          "VALUES (@id, @lastname, @firstname, @middle, @mobile, @password, @file)";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@lastname", lastname);
                        cmd.Parameters.AddWithValue("@firstname", firstname);
                        cmd.Parameters.AddWithValue("@middle", middle);
                        cmd.Parameters.AddWithValue("@mobile", mobile);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@file", imag);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                        return View();
                        }
                        else
                        {
                            // Insert failed
                            return RedirectToAction("Error", "Administration"); // Redirect to an error page or handle accordingly
                        }
                    }
                }
        }

        public ActionResult trial()
        {
            return View();
        }

        [HttpPost]
        public ActionResult trial(FormCollection collection, HttpPostedFileBase img)
        {
            if (img == null || img.ContentLength <= 0)
            {
                Response.Write("<script>alert('Please upload an image.')</script>");
                return View();
            }
            string imag = Path.GetFileName(img.FileName);
            string logpath = "c:\\Upload";  // Make sure this directory exists on your server
            string filepath = Path.Combine(logpath, imag);
            img.SaveAs(filepath);

            var id = collection["adminId"];
            var lastname = collection["lastname"];
            var firstname = collection["firstname"]; // Corrected to match your SQL query
            var middle = collection["middle"];
            var mobile = collection["mobile"];
            var password = collection["password"];

            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO ADMIN (ID, LASTNAME, FIRSTNAME, MIDDLE, MOBILE, PASSWORD, IMAGE) " +
                                      "VALUES (@id, @lastname, @firstname, @middle, @mobile, @password, @file)";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@lastname", lastname);
                    cmd.Parameters.AddWithValue("@firstname", firstname);
                    cmd.Parameters.AddWithValue("@middle", middle);
                    cmd.Parameters.AddWithValue("@mobile", mobile);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@file", imag);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Response.Write("<script>alert('Successfully Created.')</script>");
                        return View();
                    }
                    else
                    {
                        // Insert failed
                        return RedirectToAction("Error", "Administration"); // Redirect to an error page or handle accordingly
                    }
                }
            }
        }

        public ActionResult Admin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Admin(FormCollection collection, HttpPostedFileBase img)
        {
            // Check if image is uploaded
            if (img == null || img.ContentLength <= 0)
            {
                Response.Write("<script>alert('Please upload an image.')</script>");
                return View();
            }

            string imag = Path.GetFileName(img.FileName);
            string logpath = "c:\\attendance";  // Ensure this directory exists on your server
            string filepath = Path.Combine(logpath, imag);
            img.SaveAs(filepath);

            var code = collection["code"];
            var title = collection["title"];
            var description = collection["description"];
            var course_type = collection["courseType"];
            var units = collection["units"];
            var scheduleDays = collection.GetValues("schedule");
            var block = collection["block"];

            var startTimes = new List<string>();
            var endTimes = new List<string>();

            // Check if any required field is missing
            if (string.IsNullOrEmpty(code) ||
                string.IsNullOrEmpty(title) ||
                string.IsNullOrEmpty(description) ||
                string.IsNullOrEmpty(course_type) ||
                string.IsNullOrEmpty(units) ||
                scheduleDays == null ||
                scheduleDays.Length == 0 ||
                string.IsNullOrEmpty(block))
            {
                Response.Write("<script>alert('Please input all required data.')</script>");
                return View();
            }

            // Process schedule and times
            foreach (var day in scheduleDays)
            {
                var startTime = collection[$"time-{day}-start"];
                var endTime = collection[$"time-{day}-end"];

                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                {
                    Response.Write($"<script>alert('Please input both start and end times for {day}.')</script>");
                    return View();
                }

                startTimes.Add(startTime);
                endTimes.Add(endTime);
            }

            var timePairs = startTimes.Zip(endTimes, (start, end) => $"{start} - {end}");

            // Save course data to database
            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO COURSES (CODE, TITLE, DESCRIPTION, COURSE_TYPE, UNITS, SCHEDULE, TIME, BLOCK_SECTION, IMAGE_URL) " +
                                      "VALUES (@code, @title, @description, @course_type, @units, @schedule, @time, @block, @file)";
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@course_type", course_type);
                    cmd.Parameters.AddWithValue("@units", units);
                    cmd.Parameters.AddWithValue("@schedule", string.Join(", ", scheduleDays));
                    cmd.Parameters.AddWithValue("@time", string.Join(", ", timePairs));
                    cmd.Parameters.AddWithValue("@block", block);
                    cmd.Parameters.AddWithValue("@file", imag);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Successfully Created.";
                        return RedirectToAction("Admin"); // Redirect to avoid form resubmission
                    }
                    else
                    {
                        // Insert failed
                        Response.Write("<script>alert('Failed to create the course. Please try again.')</script>");
                        return View();
                    }
                }
            }
        }



        private bool CheckIfCodeExists(string code)
        {
            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) FROM COURSES WHERE CODE = @code"; // Correct table and column name
                    cmd.Parameters.AddWithValue("@code", code);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        [HttpGet]
        public FileResult Image(string filename)
        {
            var folder = "c:\\attendance";
            var filepath = Path.Combine(folder, filename);
            if (!System.IO.File.Exists(filepath))
            {
                // Return a default image or handle the error
            }
            var mime = System.Web.MimeMapping.GetMimeMapping(Path.GetFileName(filepath));
            return new FilePathResult(filepath, mime);
        }


        // UPDATE
        public ActionResult CourseUpdate()
        {
            var data = new List<object>();
            var code = Request["code"];
            var title = Request["title"];
            var course_type = Request["courseType"];
            var units = Request["units"];
            var block_section = Request["block"];
            var description = Request["description"];
            var schedule = Request["schedule"]?.Split(',') ?? new string[0];
            var time = Request["time"]?.Split(',') ?? new string[0];

            // Check if any required field is missing
            if (string.IsNullOrEmpty(code) ||
                string.IsNullOrEmpty(title) ||
                string.IsNullOrEmpty(course_type) ||
                string.IsNullOrEmpty(units) ||
                string.IsNullOrEmpty(block_section) ||
                string.IsNullOrEmpty(description) ||
                schedule.Length == 0 ||
                time.Length == 0 ||
                time.Any(t => string.IsNullOrEmpty(t) || t == " - "))
            {
                // If any required field is missing, return an error message
                data.Add(new { mess = 1, error = "Please input all required data, including schedule and time." });
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE COURSES SET " +
                                      "TITLE = @title, " +
                                      "COURSE_TYPE = @course_type, " +
                                      "UNITS = @units, " +
                                      "TIME = @time, " +
                                      "BLOCK_SECTION = @block_section, " +
                                      "DESCRIPTION = @description, " +
                                      "SCHEDULE = @schedule " +
                                      "WHERE CODE = @code";

                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@course_type", course_type);
                    cmd.Parameters.AddWithValue("@units", units);
                    cmd.Parameters.AddWithValue("@time", string.Join(", ", time));
                    cmd.Parameters.AddWithValue("@block_section", block_section);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@schedule", string.Join(",", schedule));

                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr > 0)
                    {
                        data.Add(new { mess = 0 });
                    }
                    else
                    {
                        data.Add(new { mess = 1, error = "Failed to update the course. Please try again." });
                    }
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }






        public ActionResult CourseSearch()
        {
            var data = new List<object>();
            var code = Request["code"];

            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM COURSES WHERE CODE = @code";
                    cmd.Parameters.AddWithValue("@code", code);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        var schedule = reader["schedule"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        var Time = reader["time"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        var scheduleData = schedule.Select((day, index) => new {
                            Day = day.Trim(),
                            Time = Time.Length > index ? Time[index].Trim() : string.Empty,
                        }).ToList();

                        data.Add(new
                        {
                            mess = 0,
                            code = reader["code"].ToString(),
                            title = reader["title"].ToString(),
                            course_type = reader["course_type"].ToString(),
                            units = reader["units"].ToString(),
                            schedule = scheduleData,
                            time = Time,
                            block = reader["block_section"].ToString(),
                            description = reader["description"].ToString()
                        });
                    }
                    else
                    {
                        data.Add(new { mess = 1 });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        // DELETE
        public ActionResult deleteCourse()
        {
            var data = new List<object>();
            var code = Request["code"];

            using (var db = new SqlConnection(connStr))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"DELETE FROM COURSES WHERE CODE='" + code + "'";
                    var ctr = cmd.ExecuteNonQuery();
                    if (ctr > 0)
                    {
                        data.Add(new
                        {
                            mess = 0
                        });
                    }

                }

            }
            return Json(data, JsonRequestBehavior.AllowGet);

        }
    }
}