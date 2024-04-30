using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NCKH.Models;

namespace GroupChat.Controllers
{
    public class MessageViewModel
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; } // User email
        public int GroupId { get; set; }
        public int? User_ID { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
    }
    public class ChatController : Controller
    {
        public MyDbContext db = new MyDbContext();

        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment _environment;
        public ChatController(IHubContext<ChatHub> hubContext, IWebHostEnvironment environment)
        {
            _hubContext = hubContext;
            _environment = environment;
        }

        public IActionResult Index()
        {
            string id = HttpContext.Session.GetString("user_id");

            var query = db.itemGroupChats.FromSqlRaw("SELECT GroupChat.ID, GroupChat.Name FROM AllUser JOIN User_Group ON AllUser.User_ID = User_Group.ID_UserChat " +
                                               "JOIN GroupChat ON User_Group.ID_GroupChat = GroupChat.ID " +
                                               "WHERE AllUser.User_ID = " + id);
            var result = query.ToList();

            ViewBag.CurrentUserId = int.Parse(id);

            return View("NewIndex",result);

        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(string[] checkedUsers, string GroupName)
        {

            var group = new ItemGroupChat();
            group.Name = GroupName;
            db.itemGroupChats.Add(group);
            db.SaveChanges();  // Save changes to get the auto-generated Group ID

            foreach (var userId in checkedUsers)
            {
                var userGroup = new ItemUserGroup();
                userGroup.ID_GroupChat = group.ID;
                userGroup.ID_UserChat = int.Parse(userId);

                db.itemUserGroups.Add(userGroup);
            }

            db.SaveChanges(); // Save all user group associations

            // singalR sendAll
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", group.ID, "AddUser", null, checkedUsers);


            return Ok(group.ID);
        }

        public ActionResult LoadGroupChat()
        {
            string id = HttpContext.Session.GetString("user_id");

            var query = db.itemGroupChats.FromSqlRaw("SELECT GroupChat.ID, GroupChat.Name FROM AllUser JOIN User_Group ON AllUser.User_ID = User_Group.ID_UserChat " +
                                               "JOIN GroupChat ON User_Group.ID_GroupChat = GroupChat.ID " +
                                               "WHERE AllUser.User_ID = " + id);
            var result = query.ToList();

            return PartialView("_GroupChat", result);
        }

        public ActionResult GetMessages(int groupId)
        {
			/*            var messages = db.itemMessages
										 .Where(m => m.ID_GroupChat == groupId)
										 .Join(db.AllUsers, // Join Messages with Users
											   message => message.ID_UserChat, // From Messages
											   user => user.User_ID, // From Users
											   (message, user) => new { Message = message, User = user }) // Select both
										 .Select(result => new MessageViewModel
										 {
											 ID = result.Message.ID,
											 Content = result.Message.Content,
											 UserName = result.User.Email, // Assuming you want the email
											 GroupId = result.Message.ID_GroupChat,
											 User_ID = result.Message.ID_UserChat,
											 Time = result.Message.Time,
										 })
										 .ToList();*/

			var messages = db.itemMessages
				 .Where(m => m.ID_GroupChat == groupId)
				 .GroupJoin(db.AllUsers, // Prepare for a left join with Users
					   message => message.ID_UserChat, // Key from Messages
					   user => user.User_ID, // Key from Users
					   (message, users) => new { Message = message, Users = users }) // Result selector
				 .SelectMany(
					 x => x.Users.DefaultIfEmpty(), // This ensures the left join effect
					 (x, user) => new MessageViewModel
					 {
						 ID = x.Message.ID,
						 Content = x.Message.Content,
						 UserName = user != null ? user.Email : "Unknown", // Check if user is null
						 GroupId = x.Message.ID_GroupChat,
						 User_ID = x.Message.ID_UserChat,
						 Time = x.Message.Time,
                         Type = x.Message.Type
					 })
				 .ToList();

			return PartialView("_Messages", messages);
        }

        public ActionResult GetUsersInGroup(int groupId)
        {
            var usersInGroup = db.itemUserGroups
                              .Where(ug => ug.ID_GroupChat == groupId)
                              .Join(db.AllUsers,
                                    ug => ug.ID_UserChat,
                                    u => u.User_ID,
                                    (ug, u) => u)
                              .ToList();

            return PartialView("_UsersInGroup", usersInGroup);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string Content, int GroupID)
        {
            string id = HttpContext.Session.GetString("user_id");
            string formattedDate = DateTime.Now.ToString("dd-MM HH:mm");
            var message = new ItemMessage
            {
                Content = Content,
                ID_GroupChat = GroupID,
                ID_UserChat = int.Parse(id),
                Time = formattedDate,
                Type = "text"
            };

            db.itemMessages.Add(message);
            db.SaveChanges();

            return NoContent();
        }

		[HttpPost]
		public IActionResult LeaveGroup(int GroupID)
		{
			string id = HttpContext.Session.GetString("user_id");

            try
			{
                var user = db.AllUsers.FirstOrDefault(u => u.User_ID == int.Parse(id));

                string formattedDate = DateTime.Now.ToString("dd-MM HH:mm");
                var message = new ItemMessage
                {
                    Content = user.Email + " has left the chat",
                    ID_GroupChat = GroupID,
                    ID_UserChat = null,
                    Time = formattedDate,
                    Type = "text"
                };

                db.itemMessages.Add(message);

                var row = db.itemUserGroups.FirstOrDefault(u => u.ID_GroupChat == GroupID && u.ID_UserChat == int.Parse(id));
				
				db.itemUserGroups.Remove(row);
				db.SaveChanges();

			}
			catch (Exception ex)
			{
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Status toggled successfully." });
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile file, int GroupID)
        {
            //if (file != null && file.Length > 0)
            //{
                // Prepare the destination path
                var uploadsRootFolder = Path.Combine(_environment.WebRootPath, "ImageUpload");
                if (!Directory.Exists(uploadsRootFolder))
                {
                    Directory.CreateDirectory(uploadsRootFolder);
                }

                // Ensure the filename is unique to avoid overwriting existing files
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                // Return the path to the uploaded file
                var uploadedFilePath = "/ImageUpload/" + uniqueFileName;

                string id = HttpContext.Session.GetString("user_id");
                string formattedDate = DateTime.Now.ToString("dd-MM HH:mm");
                var message = new ItemMessage
                {
                    Content = uploadedFilePath,
                    ID_GroupChat = GroupID,
                    ID_UserChat = int.Parse(id),
                    Time = formattedDate,
                    Type = "image"
                };

                db.itemMessages.Add(message);
                db.SaveChanges();


            //}

            return NoContent();
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file, int GroupID)
        {
            //if (file != null && file.Length > 0)
            //{
            // Prepare the destination path
            var uploadsRootFolder = Path.Combine(_environment.WebRootPath, "FileUpload");
            if (!Directory.Exists(uploadsRootFolder))
            {
                Directory.CreateDirectory(uploadsRootFolder);
            }

            // Ensure the filename is unique to avoid overwriting existing files
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            // Return the path to the uploaded file
            var uploadedFilePath = "/FileUpload/" + uniqueFileName;

            string id = HttpContext.Session.GetString("user_id");
            string formattedDate = DateTime.Now.ToString("dd-MM HH:mm");
            var message = new ItemMessage
            {
                Content = uploadedFilePath,
                ID_GroupChat = GroupID,
                ID_UserChat = int.Parse(id),
                Time = formattedDate,
                Type = "file"
            };

            db.itemMessages.Add(message);
            db.SaveChanges();


            //}

            return NoContent();
        }

        public ActionResult GetStudentsByClass(string classId)
        {
            List<ItemStudent> students = null;

            if (classId == "all")
            {
                students = db.Students.ToList();
            } else
            {
                students = db.Students.Where(s => s.Class_ID == int.Parse(classId)).ToList();
            }

            return PartialView("_StudentsPartial", students);

        }

        public ActionResult GetStudentsNotInGroup(string classId, string GroupID)
        {
            List<ItemStudent> students = null;

            if (classId == "all")
            {
                students = db.Students.ToList();
            }
            else
            {
                students = db.Students.Where(s => s.Class_ID == int.Parse(classId)).ToList();
            }

            var studentsInGroup = db.itemUserGroups.Where(ug => ug.ID_GroupChat == int.Parse(GroupID)).Select(ug => ug.ID_UserChat);

            students = students.Where(s => !studentsInGroup.Contains(s.User_ID)).ToList();

            return PartialView("_StudentsPartial", students);

        }

        [HttpPost]
        public async Task<IActionResult> AddToGroup(string[] checkedUsers, string GroupID)
        {

            foreach (var userId in checkedUsers)
            {

                var user = db.AllUsers.FirstOrDefault(u => u.User_ID == int.Parse(userId));

                string formattedDate = DateTime.Now.ToString("dd-MM HH:mm");
                var message = new ItemMessage
                {
                    Content = user.Email + " has join the chat",
                    ID_GroupChat = int.Parse(GroupID),
                    ID_UserChat = null,
                    Time = formattedDate,
                    Type = "text"
                };

                db.itemMessages.Add(message);

                var userGroup = new ItemUserGroup();
                userGroup.ID_GroupChat = int.Parse(GroupID);
                userGroup.ID_UserChat = int.Parse(userId);

                db.itemUserGroups.Add(userGroup);
            }

            db.SaveChanges(); // Save all user group associations

            var studentsInGroup = db.itemUserGroups.Where(ug => ug.ID_GroupChat == int.Parse(GroupID)).Select(ug => ug.ID_UserChat).ToList();

            string[] stringUserIDs = studentsInGroup.Select(id => id.ToString()).ToArray();

            // singalR sendAll
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", GroupID, "AddUser", null, stringUserIDs);


            return Ok(GroupID);
        }

    }
}
