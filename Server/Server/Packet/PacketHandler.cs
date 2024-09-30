using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;
        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;

        // 행위 자체를 Action으로 만들어서 밀어 넣어준다.
        // 이전에는 곧 바로 Room을 통해 Broadcast을 해줬는 데 
        // 이제는 해야할 일을 JobQueue에 넣어주고 하나씩 뽑아서 처리를 하는 방식으로 변경함.
        room.Push(() => room.Broadcast(clientSession, chatPacket.chat));
    }
}
