using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientSession : PacketSession
    {
        // Session ID
        public int SessionId { get; set; }
        // 현재 어떤 방에 있는지
        public GameRoom Room { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            System.Console.WriteLine($"OnConnected : {endPoint}");
            // 서버에 클라이언트가 접속을 했다면 강제로 채팅방에 들어오게 만듬
            // 하지만 실제 게임에서는 클라이언트 쪽에서 모든 리소스 업데이트가 완료 되었을 때 
            // 서버에 신호를 보내고 그때 채팅방에 들어오는 작업을 해줘야 한다.
            // To Do
            Program.Room.Push(() => Program.Room.Enter(this));
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if (Room != null)
            {
                // Room.Leave(this)이 나중에 실행 될 때 Room이 이미 null인 상태이기 때문에
                // Null Exception이 발생하게 된다.
                // Room이 null로 밀어진다 하더라도 room은 Room을 아직 참조하고 있기 때문에
                // null exception이 해결이 된다.
                GameRoom room = Room;
                room.Push(() => room.Leave(this));
                Room = null;
            }
            System.Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            System.Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
