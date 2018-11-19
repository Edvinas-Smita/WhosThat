using System;

namespace WhosThat.Recognition
{
    public class Face
    {
        public byte[] Image { get; set; }
        public int Id { get; set; }
        public String Label { get; set; }
        public int UserId { get; set; }
    }
}