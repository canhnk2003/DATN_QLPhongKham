using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GetResponseFromGemini(string userInput, ChatContext context);
    }
}
