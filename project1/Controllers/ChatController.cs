using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project1.Data;
using project1.Models;
using System.Security.Claims;

namespace project1.Controllers
{
    [Authorize(policy: "User")]
    public class ChatController : Controller
    {
        private readonly MyDbContext _dbcontext;

        public ChatController(MyDbContext context)
        {
            _dbcontext = context;
        }

        //    // =====================================
        //    // /Chat ? ????? ?? ???? ???????? ??
        //    // =====================================
        //    public IActionResult Index()
        //    {
        //        return RedirectToAction(nameof(UserChats));
        //    }

        //    // =====================================
        //    // ???? ???????? ????? ???????
        //    // =====================================
        //    public IActionResult UserChats()
        //    {
        //        int myId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        //        var chats = _dbcontext.Conversations
        //            .Where(c => c.User1Id == myId || c.User2Id == myId)
        //            .OrderByDescending(c => c.Id)
        //            .ToList();

        //        return View(chats);
        //    }

        //    // =====================================================
        //    // ????? ???? ? ???? ?? ?? ???????
        //    // =====================================================
        //    [Authorize(Roles = "User")]
        //    public IActionResult StartChatWithLibrarian()
        //    {
        //        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        //        var Librarian = _dbcontext.Users.FirstOrDefault(u => u.Role == "Librarian");
        //        if (Librarian == null)
        //            return Content("? ???????? ???? ?????");

        //        return StartChatInternal(userId, Librarian.Id);
        //    }

        //    // =====================================================
        //    // ????? ? ??????? : ???? ??????
        //    // =====================================================
        //    [Authorize(Roles = "Admin,Librarian")]
        //    public IActionResult AdminLibrarianChats()
        //    {
        //        string myRole = User.FindFirstValue(ClaimTypes.Role);

        //        var users = myRole == "Admin"
        //            ? _dbcontext.Users.Where(u => u.Role == "Librarian").ToList()
        //            : _dbcontext.Users.Where(u => u.Role == "Admin").ToList();

        //        return View(users);
        //    }

        //    // =====================================================
        //    // ???? ?? ?? ??? ?????????? (????? / ???????)
        //    // =====================================================
        //    [Authorize(Roles = "Admin,Librarian")]
        //    public IActionResult StartChat(int userId)
        //    {
        //        int myId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        //        return StartChatInternal(myId, userId);
        //    }

        //    // =====================================================
        //    // ???? ????? ???? ?? ???? ?? ?????
        //    // =====================================================
        //    private IActionResult StartChatInternal(int user1Id, int user2Id)
        //    {
        //        var conversation = _dbcontext.Conversations.FirstOrDefault(c =>
        //            (c.User1Id == user1Id && c.User2Id == user2Id) ||
        //            (c.User1Id == user2Id && c.User2Id == user1Id)
        //        );

        //        if (conversation == null)
        //        {
        //            conversation = new Conversation
        //            {
        //                User1Id = user1Id,
        //                User2Id = user2Id
        //            };

        //            _dbcontext.Conversations.Add(conversation);
        //            _dbcontext.SaveChanges();
        //        }

        //        return RedirectToAction(nameof(Conversation), new { id = conversation.Id });
        //    }

        //    // =====================================
        //    // ???? ?????
        //    // =====================================
        //    public IActionResult Conversation(int id)
        //    {
        //        int myId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        //        var conversation = _dbcontext.Conversations.FirstOrDefault(c =>
        //            c.Id == id &&
        //            (c.User1Id == myId || c.User2Id == myId)
        //        );

        //        if (conversation == null)
        //            return Unauthorized();

        //        var messages = _dbcontext.Messages
        //            .Where(m => m.ConversationId == id)
        //            .OrderBy(m => m.SentAt)
        //            .ToList();

        //        ViewBag.ConversationId = id;
        //        return View(messages);
        //    }

        //    // =====================================
        //    // ????? ????
        //    // =====================================
        //    [HttpPost]
        //    public IActionResult SendMessage(int conversationId, string text)
        //    {
        //        if (string.IsNullOrWhiteSpace(text))
        //            return RedirectToAction(nameof(Conversation), new { id = conversationId });

        //        int senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        //        var message = new Message
        //        {
        //            ConversationId = conversationId,
        //            SenderId = senderId,
        //            Text = text,
        //            SentAt = DateTime.Now
        //        };

        //        _dbcontext.Messages.Add(message);
        //        _dbcontext.SaveChanges();

        //        return RedirectToAction(nameof(Conversation), new { id = conversationId });
        //    }

        // ?? ???? ???? View (??? ????????)
        // User ? Admin ?? ??? ??????? ??????
        public IActionResult ChatWithLibrarian()
        {
            int myId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var librarian = _dbcontext.Users
                .FirstOrDefault(u => u.Role == "Librarian" && u.IsActive);

            if (librarian == null)
                return NotFound("???????? ???? ?????");

            return RedirectToAction("OpenChat", new { otherUserId = librarian.Id });
        }

        // ?? ??? ???? Librarian – ???? ??? ???????
        [Authorize(Roles = "Librarian")]
        public IActionResult Inbox()
        {
            int myId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversations = _dbcontext.Conversations
                .Include(c => c.Messages)
                .Where(c => c.User1Id == myId || c.User2Id == myId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return View(conversations); // ?? ??? View ???? ????
        }

        // ?? ??? ???? ?? ?????
        public IActionResult OpenChat(int otherUserId)
        {
            int myId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = _dbcontext.Conversations
                .Include(c => c.Messages)
                .FirstOrDefault(c =>
                    (c.User1Id == myId && c.User2Id == otherUserId) ||
                    (c.User1Id == otherUserId && c.User2Id == myId));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1Id = myId,
                    User2Id = otherUserId
                };
                _dbcontext.Conversations.Add(conversation);
                _dbcontext.SaveChanges();
            }

            ViewBag.OtherUser = _dbcontext.Users.Find(otherUserId);
            return View("Chat", conversation);
        }

        // ?? ????? ????
        [HttpPost]
        public IActionResult SendMessage(int conversationId, string text)
        {
            int senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = _dbcontext.Conversations.Find(conversationId);
            if (conversation == null)
                return NotFound();

            if (conversation.User1Id != senderId && conversation.User2Id != senderId)
                return Unauthorized();

            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Text = text
            };

            _dbcontext.Messages.Add(message);
            _dbcontext.SaveChanges();

            int otherUserId = conversation.User1Id == senderId
                ? conversation.User2Id
                : conversation.User1Id;

            return RedirectToAction("OpenChat", new { otherUserId });
        }
    }

}
