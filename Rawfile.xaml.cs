using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using XRPCLib;
using System.Text.RegularExpressions;
using System.Net;
using System.Management;
using System.Threading;

namespace Bo2All
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        XRPC remote = new XRPC();
        Random rnd = new Random();
        private static byte[] myBuffer = new byte[0x20];
        Thread t;
        public About()
        {
            InitializeComponent();
        }


        #region HUD
        public class gameAddresses
        {
            public static uint
            locString = RemoveCauseOfLeech,
            material = RemoveCauseOfLeech,
            elem = RemoveCauseOfLeech,
            type = 0x6D,
            xOffset = 0x0,
            yOffset = 0x4,
            textOffset = 0x62,
            fontSizeOffset = 12,
            colorOffset = 0x18,
            relativeOffset = 0x72,
            widthOffset = 0x58,
            heightOffset = 90,
            shaderOffset = 0x74,
            alignOffset = 0x72,
            clientOffset = 0x7C,
            sortOffset = 0x20,
            glowOffset = 0x44,
            sort = 0x32;
            public static float
            fromX = 0,
            fromY = 0,
            fromAlignOrg = 0,
            fromAlignScreen = 0;
        }

        /*
        public uint createElem(int client)
        {
            return remote.Call(gameAddresses.elem, client, 0);
        }*/

        
        public uint createElem(int client)
        {
            uint CreateElem = remote.Call(gameAddresses.elem, client, 0);
            remote.SetMemory(CreateElem + 0x7C, ReverseBytes(BitConverter.GetBytes(client)));
            return CreateElem;
        }

        public uint spawnElem(int client, uint elemAddress)
        {
            remote.SetMemory(elemAddress + 0xA8, ReverseBytes(BitConverter.GetBytes(client)));

            return elemAddress;
        }

        public uint createText(string text)
        {
            return remote.Call(gameAddresses.locString, text);
        }

        public uint getMaterialIndex(string material)
        {
            return remote.Call(gameAddresses.material, material);
        }

        public byte[] uintBytes(uint input)
        {
            byte[] data = BitConverter.GetBytes(input);
            Array.Reverse(data);
            return data;
        }

        public byte[] ReverseBytes(byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }

        public void setIcon(int client, uint elem, uint shader, int width, int height, float x, float y, uint align, float sort = 0, int r = 84, int g = 84, int b = 84, int a = 215)
        {
            remote.SetMemory(elem + gameAddresses.clientOffset, ReverseBytes(BitConverter.GetBytes((byte)client)));
            byte[] myelem = new byte[] {
                0x42, 0x48, 0x00, 0x00, 0x42, 0xF6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x29, 0xFE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x80, 0x00, 0x03, 0xFF, 0x00, 0x00, 0x01, 0xF4, 0x00, 0x00, 0x00, 0x46,
                0x00, 0x46, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x04, 0x21, 0x01, 0x00, 0x00, 0x00,
                0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00 };
            remote.SetMemory(elem, myelem);
            byte[] sorts = ReverseBytes(BitConverter.GetBytes(sort));
            remote.SetMemory(elem + gameAddresses.sort, new byte[] { sorts[0], sorts[1] });
            remote.SetMemory(elem + gameAddresses.relativeOffset, new byte[] { (byte)5 });
            remote.SetMemory(elem + gameAddresses.relativeOffset + 1, new byte[] { (byte)align });
            byte[] heights = BitConverter.GetBytes(height);
            remote.SetMemory(elem + gameAddresses.heightOffset - 2, ReverseBytes(heights));
            byte[] widths = BitConverter.GetBytes(width);
            remote.SetMemory(elem + gameAddresses.widthOffset - 2, ReverseBytes(widths));
            remote.SetMemory(elem + gameAddresses.shaderOffset, new byte[] { (byte)shader });
            remote.SetMemory(elem + gameAddresses.colorOffset, new byte[] { BitConverter.GetBytes(r)[0], BitConverter.GetBytes(g)[0], BitConverter.GetBytes(b)[0], BitConverter.GetBytes(a)[0] });
            remote.SetMemory(elem + gameAddresses.xOffset, ReverseBytes(BitConverter.GetBytes(x)));
            remote.SetMemory(elem + gameAddresses.yOffset, ReverseBytes(BitConverter.GetBytes(y)));
        }

        public void SetText(int client, uint elem, byte[] text, float fontScale, float x, float y, uint align, int r = 255, int g = 255, int b = 255, int a = 255, int GlowR = 255, int GlowG = 255, int GlowB = 255, int GlowA = 255)
        {
            remote.SetMemory(elem + 0x7c, ReverseBytes(BitConverter.GetBytes(client)));
            //byte[] myelem = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x42, 0x8F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x6B, 0x08, 0x00, 0x01, 0x80, 0x00, 0x03, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x64, 0xEA, 0x60, 0x03, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x04, 0x21, 0x00, 0x00, 0x00, 0x00, 0x01, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            //remote.SetMemory(elem, myelem);
            remote.SetMemory(elem + gameAddresses.type, new byte[] { 0x00, 0x00, 0x00, 0x01 });
            remote.SetMemory(gameAddresses.sort + elem, new byte[] { (byte)0x0A, (byte)0xFF });
            remote.SetMemory(elem + gameAddresses.textOffset, text);
            remote.SetMemory(elem + gameAddresses.relativeOffset, new byte[] { (byte)5 });
            remote.SetMemory(elem + gameAddresses.relativeOffset + 1, new byte[] { (byte)align });
            remote.SetMemory(elem + gameAddresses.textOffset + 4, new byte[] { 0x40, 0x00 });
            remote.SetMemory(elem + gameAddresses.fontSizeOffset, ReverseBytes(BitConverter.GetBytes(fontScale)));
            remote.SetMemory(elem + gameAddresses.xOffset, ReverseBytes(BitConverter.GetBytes(x)));
            remote.SetMemory(elem + gameAddresses.yOffset, ReverseBytes(BitConverter.GetBytes(y)));
            remote.SetMemory(elem + gameAddresses.colorOffset, new byte[] { BitConverter.GetBytes(r)[0], BitConverter.GetBytes(g)[0], BitConverter.GetBytes(b)[0], BitConverter.GetBytes(a)[0] });
            //remote.SetMemory(elem + gameAddresses.GlowColor, new byte[] { BitConverter.GetBytes(GlowR)[0], BitConverter.GetBytes(GlowG)[0], BitConverter.GetBytes(GlowB)[0], BitConverter.GetBytes(GlowA)[0] });
        }

        #endregion

        class Offsets
        {
            public static uint
            SENDSERVERCOMMAND = 0x8242D730,
            CBUF_ADDTEXT = 0x823FF408;
        }

        class Buttons
        {
            public static uint
            LB = 256,
            RB = 512,
            RT = 128,
            LT = 2147487744,
            LS = 1088,
            RS = 32,
            Y = 134217728,
            X = 4,
            A = 8192,
            B = 32768,
            Allonger = 32768,
            AllongerRS = 32800;
        }

        float move = -40;
        float Fx = 0;

        public void GiveDoublePoints(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x509, 1, new byte[] { 0x8D });
        }

        public void TakeDoublePoints(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x509, 1, new byte[] { 0x35 });
        }

        public void GivePerks(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x4FD, 1, new byte[] { 0xFF });
            SetMemory(xrpc_0(clientIndex) + 0x4FE, 1, new byte[] { 0x65 });
            SetMemory(xrpc_0(clientIndex) + 0x54D, 1, new byte[] { 0xFF });
            SetMemory(xrpc_0(clientIndex) + 0x54C, 1, new byte[] { 0xFF });
            SetMemory(xrpc_0(clientIndex) + 0x54B, 1, new byte[] { 0xFF });
            SetMemory(xrpc_0(clientIndex) + 0x54A, 1, new byte[] { 0xFF });
        }

        public void TakePerks(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x4FD, 1, new byte[] { 0x00 });
            SetMemory(xrpc_0(clientIndex) + 0x4FE, 1, new byte[] { 0x00 });
            SetMemory(xrpc_0(clientIndex) + 0x54D, 1, new byte[] { 0x00 });
            SetMemory(xrpc_0(clientIndex) + 0x54C, 1, new byte[] { 0x00 });
            SetMemory(xrpc_0(clientIndex) + 0x54B, 1, new byte[] { 0x00 });
            SetMemory(xrpc_0(clientIndex) + 0x54A, 1, new byte[] { 0x00 });
        }

        public uint xrpc_2(uint clientIndex)
        {
            return(BitConverter.ToUInt32(remote.GetMemory(xrpc_0(clientIndex) + 0x568C, 4), 0)); //Clean version of buttons monitoring
        }

        public void setPulseFx(uint elem, int client)
        {
            for (; ; )
            {
                if(Fx == 0)
                    Fx += 5f;
                WriteFloat(elem + gameAddresses.heightOffset, Fx);
                spawnElem(client, elem);
                if (Fx == 15)
                    Fx -= 5f;
            }
        }

        public void Move(uint elem, int client)
        {
            for(;   ;)
            {
                move += 5f; //Speed
                WriteFloat(elem + gameAddresses.xOffset, move); //minimize
                spawnElem(client, elem);
                if (move == 1600) //if position of text == 1400 so return to -5
                    move = -40;
            }
        }

        public void StartfunnyClantag(uint clientIndex)
        {
            Clantag = new Thread(() => FunnyClantag(clientIndex));
            Clantag.Start();
        }

        void FunnyClantag(uint clientIndex)
        {
            while(true)
            {
                WriteString(xrpc_0(clientIndex) + 0x55A0, "^1EC^7"); Thread.Sleep(50);
                WriteString(xrpc_0(clientIndex) + 0x55A0, "^2EC^7"); Thread.Sleep(50);
                WriteString(xrpc_0(clientIndex) + 0x55A0, "^3EC^7"); Thread.Sleep(50);
                WriteString(xrpc_0(clientIndex) + 0x55A0, "^5EC^7"); Thread.Sleep(50);
                WriteString(xrpc_0(clientIndex) + 0x55A0, "^6EC^7"); Thread.Sleep(50);
                WriteString(xrpc_0(clientIndex) + 0x55A0, "^3EC^7"); Thread.Sleep(50);
                WriteString(xrpc_0(clientIndex) + 0x55A0, "^5EC^7"); Thread.Sleep(50);
            }
        }

        void Newsbar()
        {
            for (int j = 0; j <= 17; j++)
            {
                    elemshader = createElem(1);
                    shader = getMaterialIndex("black");
                    setIcon(1, elemshader, shader, 2000, 50, 320, 400, 1, 0, 2);
                    spawnElem(1, elemshader);
                    elem = createElem(j);
                    SetText(j, elem, uintBytes(createText(elemtext)), 10, 10, 435, 0, 255);
                    spawnElem(j, elem);
                Move(elem, j);
            }
        }

        public String ReadString(UInt32 Address, UInt32 MaxLength)
            {
                Byte[] Data;
                UInt32 BytesRead;
                Int32 i, Length = 0;
                Data = new Byte[MaxLength];
                remote.xbCon.DebugTarget.GetMemory(Address, MaxLength, Data, out BytesRead);
                for (i = 0; i < MaxLength && Data[i] > 0; i++, Length++) ;
                return Encoding.ASCII.GetString(Data, 0, Length);
            }

        public void WriteString(UInt32 Address, String Value)
            {
                Byte[] Data;
                Int32 Length;
                UInt32 BytesWritten;
                Length = Value.Length + 1;
                Data = new Byte[Length];
                Encoding.ASCII.GetBytes(Value, 0, Value.Length, Data, 0);
                remote.xbCon.DebugTarget.SetMemory(Address, (UInt32)Length, Data, out BytesWritten);
            }

        public void WriteFloat(uint offset, float input)
            {
                    BitConverter.GetBytes(input).CopyTo(myBuffer, 0);
                    Array.Reverse(myBuffer, 0, 4);
                    uint outInt;
                    remote.xbCon.DebugTarget.SetMemory(offset, 4, myBuffer, out outInt);
            }

        public float ReadFloat(uint offset)
            {
                    uint outInt;
                    remote.xbCon.DebugTarget.GetMemory(offset, 4, myBuffer, out outInt);
                    Array.Reverse(myBuffer, 0, 4);
                    return BitConverter.ToSingle(myBuffer, 0);
            }

        public byte ReadByte(uint offset)
        {
            uint Index;
            remote.xbCon.DebugTarget.GetMemory(offset, 1, myBuffer, out Index);
            return myBuffer[0];
        }

        private uint ReadUInt32(uint offset)
        {
            uint outInt;
            remote.xbCon.DebugTarget.GetMemory(offset, 4, myBuffer, out outInt);
            Array.Reverse(myBuffer, 0, 4);
            return BitConverter.ToUInt32(myBuffer, 0);
        }

        public string[] Client = new string[] { "Client : 0", "Client : 1", "Client : 2", "Client : 3", "Client : 4", "Client : 5", "Client : 6",
        "Client : 7", "Client : 8", "Client : 9", "Client : 10", "Client : 11", "Client : 12", "Client : 13", "Client : 14", "Client : 15", "Client : 16", "Client : 17"};

        public string[] Colours = new string[] { "^1", "^2", "^3", "^4", "^5", "^6", "^7", "^8", "^9" };

        public float[] Score = new float[2] { 13370000f, 0.0f };

        public bool[] Check = new bool[2] { true, false };

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        void anim_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetMemory(uint Address, uint BytesToWrite, byte[] D)
        {
            uint Index;
            remote.xbCon.DebugTarget.SetMemory(Address, BytesToWrite, D, out Index);
        }

        public void GetMemory(uint Address, uint BytesToRead, byte[] D, bool boonlean, uint buffer)
        {
            uint Index;
            remote.xbCon.DebugTarget.GetMemory(Address, BytesToRead, D, out Index);
            remote.xbCon.DebugTarget.InvalidateMemoryCache(boonlean, Address, buffer);
        }

        public void printf(string msg)
        {
        }

        public float ScoreNormal = 0;

        public void MaxScore(uint clientIndex)
        {
            ScoreNormal = ReadFloat(xrpc_0(clientIndex) + 0x55C8);
            WriteFloat(xrpc_0(clientIndex) + 0x55C8, Score[0]);
        }

        public void MinScore(uint clientIndex)
        {
            ScoreNormal = ReadFloat(xrpc_0(clientIndex) + 0x55C8);
            WriteFloat(xrpc_0(clientIndex) + 0x55C8, Score[1]);
        }

        public void ResetScoreToNormal(uint clientIndex)
        {
            WriteFloat(xrpc_0(clientIndex) + 0x55C8, ScoreNormal);
        }

        public void Gravity(uint clientIndex)
        {
            IsGravity = ReadUInt32(xrpc_0(clientIndex) + 0x1B);
            if (IsGravity == 0x85) { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x75 }); }
            else { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x25 }); }
        }

        public void GravityOff(uint clientIndex)
        {
            IsGravity = ReadUInt32(xrpc_0(clientIndex) + 0x1B);
            if (IsGravity == 0x25) { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x00 }); }
            else { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x85 }); }
        }

        public void HideWeapon_MP(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x17, 1, new byte[] { 0x80 });
        }

        public void HideWeaponOff_MP(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x17, 1, new byte[] { 0x00 });
        }

        public void CheckerboardScreen(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x1A, 1, new byte[] { 0xFF });
        }

        public void CheckerboardScreenOff(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x1A, 1, new byte[] { 0x00 });
        }

        public void DisabledJump(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0xD, 1, new byte[1] { 0x78 });
        }

        public void EnableJump(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0xD, 1, new byte[1] { 0x00 });
        }

        public void DisabledSprint(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0xD, 1, new byte[1] { 0x95 });
        }

        public void EnableSprint(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0xD, 1, new byte[1] { 0x00 });
        }

        public void DisabledWeapon(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x17, 1, new byte[1] { 0xFF });
        }

        public void EnableWeapon(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x17, 1, new byte[1] { 0x00 });
        }

        public void InvisibilityOn(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0xFF, 1, new byte[1] { 0xFF });
        }

        public void InvisibilityOff(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0xFF, 1, new byte[1] { 0x00 });
        }

        public void CameleonOn(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x1A, 1, new byte[1] { 0xFF });
        }

        public void CameleonOff(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x1A, 1, new byte[1] { 0x00 });
        }

        public void MaxAmmo(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x43A, 1, new byte[] { 0xFF });
            SetMemory(xrpc_0(clientIndex) + 0x43E, 1, new byte[] { 0xFF });
        }

        public void MaxAmmoOff(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x43A, 1, new byte[] { 0x00 });
            SetMemory(xrpc_0(clientIndex) + 0x43E, 1, new byte[] { 0x00 });
        }

        public void MaxAmmo_ZM(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x436, 1, new byte[] { 0xFF });
            SetMemory(xrpc_0(clientIndex) + 0x42A, 1, new byte[] { 0xFF });
            SetMemory(xrpc_0(clientIndex) + 0x42E, 1, new byte[] { 0xFF });
        }

        public void MaxAmmoOff_ZM(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x436, 1, new byte[] { 0x00 });
            SetMemory(xrpc_0(clientIndex) + 0x42A, 1, new byte[] { 0x00 });
            SetMemory(xrpc_0(clientIndex) + 0x42E, 1, new byte[] { 0x00 });
        }

        public void ThirdPerson(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x84, 1, new byte[] { 0xFF });
        }

        public void ThirdPersonOff(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x84, 1, new byte[] { 0x00 });
        }

        public void SuperSpeed(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x54E1, 4, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
        }

        public void SuperSpeedOff(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x54E1, 4, new byte[] { 0x80, 0x00, 0x00, 0x00 });
        }

        public void God(uint clientIndex)
        {
            IsEmp = ReadUInt32(xrpc_0(clientIndex) + 0x1B);
            if (IsEmp == 1207959552) { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x75 }); }
            else { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x85 }); }
        }

        public void God_ZM(uint clientIndex)
        {
            IsGravity = ReadUInt32(xrpc_0(clientIndex) + 0x1B);
            if (IsEmp == 620756992) { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x78 }); }
            else { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x85 }); }
        }

        public void GodOff(uint clientIndex)
        {
            IsEmp = ReadUInt32(xrpc_0(clientIndex) + 0x1B);
            if (IsEmp == 1962934272) { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x48 }); }
            else { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x00 }); }
        }

        uint IsGod = 0;
        uint IsEmp = 0;
        uint IsGravity = 0;

        public void EMP(uint clientIndex)
        {
            IsGod = ReadUInt32(xrpc_0(clientIndex) + 0x1B);
            if (IsGod == 2231369728) { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x75 }); }
            else { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x48 }); }
        }

        public void EMPOff(uint clientIndex)
        {
            IsGod = ReadUInt32(xrpc_0(clientIndex) + 0x1B);
            if (IsGod == 1962934272) { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x85 }); }
            else { SetMemory(xrpc_0(clientIndex) + 0x1B, 1, new byte[] { 0x00 }); }
        }

        public void RedBoxesFreez(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x100, 1, new byte[] { 0x10 });
        }

        public void RedBoxesFreezOff(uint clientIndex)
        {
            SetMemory(xrpc_0(clientIndex) + 0x100, 1, new byte[] { 0x00 });
        }

        uint DefaultWeapon_zmp = 0x1;
        uint Minigun_zm = 0x33;
        uint Raygun_zm = 0x31;
        uint Fuckyouiamwizard_zm = 0x54;

        public void GiveWeapon_ZM(uint clientIndex, string WEAP_NAME_ZM)
        {
            switch (WEAP_NAME_ZM)
            {
                case "DEFAULTWEAPON_ZM":
                    SetMemory(xrpc_0(clientIndex) + 0x24B, 1, new byte[] { (byte)DefaultWeapon_zmp });
                    break;
                case "MINIGUN_ZM":
                    SetMemory(xrpc_0(clientIndex) + 0x24B, 1, new byte[] { (byte)Minigun_zm });
                    break;
                case "RAYGUN_ZM":
                    SetMemory(xrpc_0(clientIndex) + 0x24B, 1, new byte[] { (byte)Raygun_zm });
                    break;
                case "FUCKYOUIAMWIZARD_ZM":
                    SetMemory(xrpc_0(clientIndex) + 0x24B, 1, new byte[] { (byte)Fuckyouiamwizard_zm });
                    break;
            }
        }

        public void GiveWeapon_MP(uint clientIndex, string WEAP_NAME_MP)
        {
            switch (WEAP_NAME_MP)
            {
                case "AN94_MP":
                    SetMemory(xrpc_0(clientIndex) + 0x2BB, 1, new byte[] { 0x30 });
                    break;
                case "ARROW_MP":
                    SetMemory(xrpc_0(clientIndex) + 0x2BB, 1, new byte[] { 0x70 });
                    xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Use it like a frag.\"");
                    break;
                case "RPG_MP":
                    SetMemory(xrpc_0(clientIndex) + 0x2BB, 1, new byte[] { 0x56 });
                    break;
                case "MP7_MP":
                    SetMemory(xrpc_0(clientIndex) + 0x2BB, 1, new byte[] { 0x02 });
                    break;
            }
        }

        public string CurrentGT = "";

        public void ColourGt(uint clientIndex, bool stop)
        {
            CurrentGT = ReadString(xrpc_0(clientIndex) + 0x5534, 15);
            int colours = rnd.Next(0, 10);
            switch (colours)
            {
                case 1:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[0] + CurrentGT);
                    break;
                case 2:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[1] + CurrentGT);
                    break;
                case 3:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[2] + CurrentGT);
                    break;
                case 4:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[3] + CurrentGT);
                    break;
                case 5:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[4] + CurrentGT);
                    break;
                case 6:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[5] + CurrentGT);
                    break;
                case 7:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[6] + CurrentGT);
                    break;
                case 8:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[7] + CurrentGT);
                    break;
                case 9:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[8] + CurrentGT);
                    break;
                case 10:
                    WriteString(xrpc_0(clientIndex) + 0x5534, Colours[9] + CurrentGT);
                    break;
            }
            if (stop)
            {
                WriteString(xrpc_0(clientIndex) + 0x5534, CurrentGT);
            }
        }

        private void Label_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void Label_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void TabItem_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MenuItem_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            
        }

        public void Noclip()
        {
            int Time = 0;
            SetMemory(0x8354967A, 1, new byte[] { 0x00 });
            while (true)
            {
                SetMemory(0x835495A3, 1, new byte[] { 0x1 });
                xrpc_1(Offsets.SENDSERVERCOMMAND, 0, 0, "< \"Initializing...\"");
                Thread.Sleep(Time += 500);
                if (Time == 2000)
                    break;
            }
            SetMemory(0x835495A3, 1, new byte[] { 0x2 });
            xrpc_1(Offsets.SENDSERVERCOMMAND, 0, 0, "< \"Noclipin!\"");
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            remote.Connect();
            if (remote.activeConnection)
            {
                statuslabel1.Content = "Connected to : " + remote.xbCon.Name;
                this.Title = "Eclipse - Running " + remote.xbCon.Name;
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            int random1 = rnd.Next(1, 9999999);
            remote.xbCon.ScreenShot("myScreenShot-" + random1 + ".bmp");
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        public string ByteToString(byte[] input)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(input);
        }

        public uint xrpc_0(uint clientIndex)
        {
            return (0x83544190 + ((uint)clientIndex * 0x57F8));
        }

        float JetPack = (float)0.0f;
        float count = (float)2.5f;

        void optimizedJetpack(uint clientIndex)
        {
        }

        public void xrpc_1(uint Offset, params object[] args)
        {
            remote.Call(Offset, args);
        }

        public string str = "";

        public string GetGamertag(uint clientIndex)
        {
	        uint num;
	        byte[] data = new byte[20];
	        remote.xbCon.DebugTarget.GetMemory(xrpc_0(clientIndex) + 0x5534, 20, data, out num); remote.xbCon.DebugTarget.InvalidateMemoryCache(true, xrpc_0(clientIndex) + 0x5534, 20);
	        str = RemoveSpecialCharacters(ByteToString(data));
	        if (str == "")
            {
		        str = "";
            }
            return str;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[0];
            CHECK_25.IsEnabled = Check[0];
        }

        private void ListBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[1]; CHECK_25.IsEnabled = Check[0];
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            listitem1.Content = (string)GetGamertag(0);
            listbox2.Content = (string)GetGamertag(1);
            listitem3.Content = (string)GetGamertag(2);
            listitem4.Content = (string)GetGamertag(3);
            listitem5.Content = (string)GetGamertag(4);
            listitem6.Content = (string)GetGamertag(5);
            listitem7.Content = (string)GetGamertag(6);
            listitem8.Content = (string)GetGamertag(7);
            listitem9.Content = (string)GetGamertag(8);
            listitem10.Content = (string)GetGamertag(9);
            listitem11.Content = (string)GetGamertag(10);
            listitem12.Content = (string)GetGamertag(11);
            listitem13.Content = (string)GetGamertag(12);
            listitem14.Content = (string)GetGamertag(13);
            listitem15.Content = (string)GetGamertag(14);
            listitem16.Content = (string)GetGamertag(15);
            listitem17.Content = (string)GetGamertag(16);
            listitem18.Content = (string)GetGamertag(17);
        }

        private void listitem3_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[2]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem4_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[3]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem5_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[4]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem6_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[5]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem7_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[6]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem8_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[7]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem9_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[8]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem10_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[9]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem11_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[10]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem12_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[11]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem13_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[12]; CHECK_25.IsEnabled = Check[0];

        }

        private void listitem14_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[13]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem15_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[14]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem16_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[15]; CHECK_25.IsEnabled = Check[0];
        }

        private void listitem17_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[16]; CHECK_25.IsEnabled = Check[0];

        }

        private void listitem18_Selected(object sender, RoutedEventArgs e)
        {
            statuslabel2.Content = Client[17]; CHECK_25.IsEnabled = Check[0];
        }

        public float X = 0; public float Y = 0; public float Z = 0;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            for(uint j = 0; j <= 0x11; j++)
            {
                WriteFloat(xrpc_0(j) + 0x24, X);
                WriteFloat(xrpc_0(j) + 0x28, Y);
                WriteFloat(xrpc_0(j) + 0x2c, Z);
            }
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            if (statuslabel2.Content == Client[0])
            {
                DisabledJump(0);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 0, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[1])
            {
                DisabledJump(1);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 1, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[2])
            {
                DisabledJump(2);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 2, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[3])
            {
                DisabledJump(3);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 3, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[4])
            {
                DisabledJump(4);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 4, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[5])
            {
                DisabledJump(5);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 5, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[6])
            {
                DisabledJump(6);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 6, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[7])
            {
                DisabledJump(7);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 7, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[8])
            {
                DisabledJump(8);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 8, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[9])
            {
                DisabledJump(9);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 9, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[10])
            {
                DisabledJump(10);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 10, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[11])
            {
                DisabledJump(11);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 11, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[12])
            {
                DisabledJump(12);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 12, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[13])
            {
                DisabledJump(13);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 13, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[14])
            {
                DisabledJump(14);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 14, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[15])
            {
                DisabledJump(15);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 15, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[16])
            {
                DisabledJump(16);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 16, 0, "; \"^3DisabledJump ^2On\"");
            }
            if (statuslabel2.Content == Client[17])
            {
                DisabledJump(17);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 17, 0, "; \"^3DisabledJump ^2On\"");
            }
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            if (statuslabel2.Content == Client[0])
            {
                EnableJump(0);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 0, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[1])
            {
                EnableJump(1);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 1, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[2])
            {
                EnableJump(2);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 2, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[3])
            {
                EnableJump(3);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 3, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[4])
            {
                EnableJump(4);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 4, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[5])
            {
                EnableJump(5);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 5, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[6])
            {
                EnableJump(6);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 6, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[7])
            {
                EnableJump(7);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 7, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[8])
            {
                EnableJump(8);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 8, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[9])
            {
                EnableJump(9);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 9, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[10])
            {
                EnableJump(10);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 10, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[11])
            {
                EnableJump(11);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 11, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[12])
            {
                EnableJump(12);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 13, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[13])
            {
                EnableJump(13);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 13, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[14])
            {
                EnableJump(14);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 14, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[15])
            {
                EnableJump(15);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 15, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[16])
            {
                EnableJump(16);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 16, 0, ": \"^3DisabledJump ^1Off\"");
            }
            if (statuslabel2.Content == Client[17])
            {
                EnableJump(17);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 17, 0, ": \"^3DisabledJump ^1Off\"");
            }
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            if (statuslabel2.Content == Client[0])
            {
                DisabledSprint(0);
                xrpc_1(Offsets.SENDSERVERCOMMAND, 0, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[1])
            {
                DisabledSprint(1); xrpc_1(Offsets.SENDSERVERCOMMAND, 1, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[2])
            {
                DisabledSprint(2); xrpc_1(Offsets.SENDSERVERCOMMAND, 2, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[3])
            {
                DisabledSprint(3); xrpc_1(Offsets.SENDSERVERCOMMAND, 3, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[4])
            {
                DisabledSprint(4); xrpc_1(Offsets.SENDSERVERCOMMAND, 4, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[5])
            {
                DisabledSprint(5); xrpc_1(Offsets.SENDSERVERCOMMAND, 5, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[6])
            {
                DisabledSprint(6); xrpc_1(Offsets.SENDSERVERCOMMAND, 6, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[7])
            {
                DisabledSprint(7); xrpc_1(Offsets.SENDSERVERCOMMAND, 7, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[8])
            {
                DisabledSprint(8); xrpc_1(Offsets.SENDSERVERCOMMAND, 8, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[9])
            {
                DisabledSprint(9); xrpc_1(Offsets.SENDSERVERCOMMAND, 9, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[10])
            {
                DisabledSprint(10); xrpc_1(Offsets.SENDSERVERCOMMAND, 10, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[11])
            {
                DisabledSprint(11); xrpc_1(Offsets.SENDSERVERCOMMAND, 11, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[12])
            {
                DisabledSprint(12); xrpc_1(Offsets.SENDSERVERCOMMAND, 12, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[13])
            {
                DisabledSprint(13); xrpc_1(Offsets.SENDSERVERCOMMAND, 13, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[14])
            {
                DisabledSprint(14); xrpc_1(Offsets.SENDSERVERCOMMAND, 14, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[15])
            {
                DisabledSprint(15); xrpc_1(Offsets.SENDSERVERCOMMAND, 15, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[16])
            {
                DisabledSprint(16); xrpc_1(Offsets.SENDSERVERCOMMAND, 16, 0, "; \"^3DisabledSprint ^2On\"");
            }
            if (statuslabel2.Content == Client[17])
            {
                DisabledSprint(17); xrpc_1(Offsets.SENDSERVERCOMMAND, 17, 0, "; \"^3DisabledSprint ^2On\"");
            }
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            if (statuslabel2.Content == Client[0])
            {
                EnableSprint(0); xrpc_1(Offsets.SENDSERVERCOMMAND, 0, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[1])
            {
                EnableSprint(1); xrpc_1(Offsets.SENDSERVERCOMMAND, 1, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[2])
            {
                EnableSprint(2); xrpc_1(Offsets.SENDSERVERCOMMAND, 2, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[3])
            {
                EnableSprint(3); xrpc_1(Offsets.SENDSERVERCOMMAND, 3, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[4])
            {
                EnableSprint(4); xrpc_1(Offsets.SENDSERVERCOMMAND, 4, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[5])
            {
                EnableSprint(5); xrpc_1(Offsets.SENDSERVERCOMMAND, 5, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[6])
            {
                EnableSprint(6); xrpc_1(Offsets.SENDSERVERCOMMAND, 6, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[7])
            {
                EnableSprint(7); xrpc_1(Offsets.SENDSERVERCOMMAND, 7, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[8])
            {
                EnableSprint(8);xrpc_1(Offsets.SENDSERVERCOMMAND, 8, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[9])
            {
                EnableSprint(9); xrpc_1(Offsets.SENDSERVERCOMMAND, 9, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[10])
            {
                EnableSprint(10); xrpc_1(Offsets.SENDSERVERCOMMAND, 10, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[11])
            {
                EnableSprint(11); xrpc_1(Offsets.SENDSERVERCOMMAND, 11, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[12])
            {
                EnableSprint(12); xrpc_1(Offsets.SENDSERVERCOMMAND, 12, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[13])
            {
                EnableSprint(13); xrpc_1(Offsets.SENDSERVERCOMMAND, 13, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[14])
            {
                EnableSprint(14); xrpc_1(Offsets.SENDSERVERCOMMAND, 14, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[15])
            {
                EnableSprint(15); xrpc_1(Offsets.SENDSERVERCOMMAND, 15, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[16])
            {
                EnableSprint(16); xrpc_1(Offsets.SENDSERVERCOMMAND, 16, 0, "; \"^3DisabledSprint ^1Off\"");
            }
            if (statuslabel2.Content == Client[17])
            {
                EnableSprint(17); xrpc_1(Offsets.SENDSERVERCOMMAND, 17, 0, "; \"^3DisabledSprint ^1Off\"");
            }
        }

        private void checkBox3_Checked(object sender, RoutedEventArgs e)
        {
            DisabledWeapon(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3DisabledWeapon ^2On\"");
        }

        private void checkBox3_Unchecked(object sender, RoutedEventArgs e)
        {
            EnableWeapon(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3DisabledWeapon ^1Off\"");
        }

        private void checkBox4_Checked(object sender, RoutedEventArgs e)
        {
            InvisibilityOn(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Invisibility ^2On\"");          
        }

        private void checkBox4_Unchecked(object sender, RoutedEventArgs e)
        {
            InvisibilityOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Invisibility ^1Off\"");
            
        }

        private void SendServer_1_Click(object sender, RoutedEventArgs e)
        {
            if (statuslabel2.Content == Client[0])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 0, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[1])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 1, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[2])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 2, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[3])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 3, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[4])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 4, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[5])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 5, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[6])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 6, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[7])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 7, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[8])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 8, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[9])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 9, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[10])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 10, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[11])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 11, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[12])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 12, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[13])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 13, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[14])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 14, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[15])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 15, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[16])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 16, 0, SendServer_0.Text);
            }
            if (statuslabel2.Content == Client[17])
            {
                xrpc_1(Offsets.SENDSERVERCOMMAND, 17, 0, SendServer_0.Text);
            }
        }

        uint elemshader = 0;
        uint shader = 0;
        uint elem = 0;
        string elemtext = "";

        private void Cbuf_1_Click(object sender, RoutedEventArgs e)
        {
            Thread n = new Thread(Newsbar);
            elemtext = HUD_0.Text;
            n.Start();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            X = ReadFloat(xrpc_0(Convert.ToUInt32(listBox1.SelectedIndex)) + 0x24);
            Y = ReadFloat(xrpc_0(Convert.ToUInt32(listBox1.SelectedIndex)) + 0x28);
            Z = ReadFloat(xrpc_0(Convert.ToUInt32(listBox1.SelectedIndex)) + 0x2c);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void checkBox10_Checked(object sender, RoutedEventArgs e)
        {
            MaxAmmo(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3MaxAmmo ^2On\"");
        }

        private void checkBox10_Unchecked(object sender, RoutedEventArgs e)
        {
            MaxAmmoOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3MaxAmmo ^1Off\"");
        }

        private void checkBox7_Checked(object sender, RoutedEventArgs e)
        {
            CameleonOn(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Cameleon ^2On\"");
        }

        private void checkBox7_Unchecked(object sender, RoutedEventArgs e)
        {
            CameleonOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Cameleon ^1Off\"");
        }

        private void checkBox11_Checked(object sender, RoutedEventArgs e)
        {
            ThirdPerson(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3ThirdPerson ^2On\"");
        }

        private void checkBox11_Unchecked(object sender, RoutedEventArgs e)
        {
            ThirdPersonOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3ThirdPerson ^1Off\"");
        }

        private void checkBox6_Checked(object sender, RoutedEventArgs e)
        {
            SuperSpeed(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3SuperSpeed ^2On\"");
        }

        private void checkBox6_Unchecked(object sender, RoutedEventArgs e)
        {
            SuperSpeedOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3SuperSpeed ^1Off\"");
        }

        private void checkBox12_Checked(object sender, RoutedEventArgs e)
        {
            God(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3God ^2On\"");
        }

        private void checkBox12_Unchecked(object sender, RoutedEventArgs e)
        {
            GodOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3God ^1Off\"");
        }

        private void checkBox9_Checked(object sender, RoutedEventArgs e)
        {
            RedBoxesFreez(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3RedBoxesFreez ^2On\"");
        }

        private void checkBox9_Unchecked(object sender, RoutedEventArgs e)
        {
            RedBoxesFreezOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3RedBoxesFreez ^1Off\""); 
        }

        private void Gt_1_Click(object sender, RoutedEventArgs e)
        {
            WriteString(xrpc_0(Convert.ToUInt32(listBox1.SelectedIndex)) + 0x5534, Gt_0.Text);
        }

        public float MultiX = 0; public float MultiY = 0; public float MultiZ = 0;

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void checkBox5_Checked(object sender, RoutedEventArgs e)
        {
            EMP(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3EMP Jammed ^2On\"");
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            WriteFloat(0x82084564, Convert.ToUInt32(Jump_0.Value));
            xrpc_1(Offsets.SENDSERVERCOMMAND, -1, 0, "; \"jump_height set to " + Jump_0.Value + "\"");
        }

        private void Fall_1_Click(object sender, RoutedEventArgs e)
        {
            WriteFloat(0x82012670, Convert.ToUInt32(Fall_0.Value));
            WriteFloat(0x82003EF4, Convert.ToUInt32(Fall_0.Value));
            WriteFloat(0x8200457C, Convert.ToUInt32(Fall_0.Value));
            xrpc_1(Offsets.SENDSERVERCOMMAND, -1, 0, "; \"bg_minfalldamage set to " + Fall_0.Value + "\"");
        }

        private void checkBox13_Checked(object sender, RoutedEventArgs e)
        {
            God(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3God ^2On\"");
        }

        private void checkBox13_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void checkBox13_Unchecked(object sender, RoutedEventArgs e)
        {
            GodOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3God ^1Off\"");
        }

        private void checkBox15_Checked(object sender, RoutedEventArgs e)
        {
            SuperSpeed(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3SuperSpeed ^2On\"");
        }

        private void checkBox15_Unchecked(object sender, RoutedEventArgs e)
        {
            SuperSpeedOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3SuperSpeed ^1Off\"");
        }

        private void checkBox18_Checked(object sender, RoutedEventArgs e)
        {
            Gravity(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Gravity ^2On\"");
        }

        private void checkBox21_Checked(object sender, RoutedEventArgs e)
        {
            MaxScore(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Score set to Max\"");
        }

        private void checkBox21_Unchecked(object sender, RoutedEventArgs e)
        {
            ResetScoreToNormal(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Score Reset!\"");
        }

        private void checkBox22_Checked(object sender, RoutedEventArgs e)
        {
            MinScore(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Score set to Min\"");
        }

        private void checkBox22_Unchecked(object sender, RoutedEventArgs e)
        {
            ResetScoreToNormal(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Score Reset!\"");
        }

        private void Score_1_Click(object sender, RoutedEventArgs e)
        {
            WriteFloat(xrpc_0(Convert.ToUInt32(listBox1.SelectedIndex)) + 0x55C8, Convert.ToUInt32(Score_0.Value));
        }

        private void checkBox18_Unchecked(object sender, RoutedEventArgs e)
        {
            GravityOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Gravity ^1Off\"");
        }

        private void checkBox16_Checked(object sender, RoutedEventArgs e)
        {
            CheckerboardScreen(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3CheckerboardScreen ^2On\"");
        }

        private void checkBox16_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckerboardScreenOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3CheckerboardScreen ^1Off\"");
        }

        private void checkBox13_Checked_1(object sender, RoutedEventArgs e)
        {
            God(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3God ^2On\"");
        }

        private void checkBox13_Unchecked_1(object sender, RoutedEventArgs e)
        {
            GodOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3God ^2Off\"");
        }

        private void checkBox24_Checked(object sender, RoutedEventArgs e)
        {
            MaxAmmo_ZM(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3MaxAmmo ^2On\"");
        }

        private void checkBox24_Unchecked(object sender, RoutedEventArgs e)
        {
            MaxAmmoOff_ZM(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3MaxAmmo ^1Off\"");
        }

        private void WEAP_ZM_0_Checked(object sender, RoutedEventArgs e)
        {
            GiveWeapon_ZM(Convert.ToUInt32(listBox1.SelectedIndex), "DEFAULTWEAPON_ZM");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Weapon Given!\"");
            WEAP_ZM_1.IsChecked = null;
            WEAP_ZM_2.IsChecked = null;
            WEAP_ZM_3.IsChecked = null;
        }

        private void WEAP_ZM_1_Checked(object sender, RoutedEventArgs e)
        {
            GiveWeapon_ZM(Convert.ToUInt32(listBox1.SelectedIndex), "MINIGUN_ZM");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Weapon Given!\"");
            WEAP_ZM_0.IsChecked = null;
            WEAP_ZM_2.IsChecked = null;
            WEAP_ZM_3.IsChecked = null;
        }

        private void WEAP_ZM_2_Checked(object sender, RoutedEventArgs e)
        {
            GiveWeapon_ZM(Convert.ToUInt32(listBox1.SelectedIndex), "RAYGUN_ZM");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Weapon Given!\"");
            WEAP_ZM_0.IsChecked = null;
            WEAP_ZM_1.IsChecked = null;
            WEAP_ZM_3.IsChecked = null;
        }

        private void WEAP_ZM_3_Checked(object sender, RoutedEventArgs e)
        {
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Weapon Given!\"");
            WEAP_ZM_0.IsChecked = null;
            WEAP_ZM_1.IsChecked = null;
            WEAP_ZM_2.IsChecked = null;
        }

        private void WEAP_MP_0_Checked(object sender, RoutedEventArgs e)
        {
            GiveWeapon_MP(Convert.ToUInt32(listBox1.SelectedIndex), "AN94_MP");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Weapon Given!\"");
            WEAP_MP_1.IsChecked = null;
            WEAP_MP_2.IsChecked = null;
            WEAP_MP_3.IsChecked = null;
        }

        private void WEAP_MP_1_Checked(object sender, RoutedEventArgs e)
        {
            GiveWeapon_MP(Convert.ToUInt32(listBox1.SelectedIndex), "ARROW_MP");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Weapon Given!\"");
            WEAP_MP_0.IsChecked = null;
            WEAP_MP_2.IsChecked = null;
            WEAP_MP_3.IsChecked = null;
        }

        private void WEAP_MP_2_Checked(object sender, RoutedEventArgs e)
        {
            GiveWeapon_MP(Convert.ToUInt32(listBox1.SelectedIndex), "MP7_MP");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Weapon Given!\"");
            WEAP_MP_0.IsChecked = null;
            WEAP_MP_1.IsChecked = null;
            WEAP_MP_3.IsChecked = null;
        }

        private void WEAP_MP_3_Checked(object sender, RoutedEventArgs e)
        {
            GiveWeapon_MP(Convert.ToUInt32(listBox1.SelectedIndex), "RPG_MP");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Weapon Given!\"");
            WEAP_MP_1.IsChecked = null;
            WEAP_MP_2.IsChecked = null;
            WEAP_MP_0.IsChecked = null;
        }

        private void checkBox5_Unchecked(object sender, RoutedEventArgs e)
        {
            EMPOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3EMP Jammed ^1Off\"");
        }

        public string CurrentClantag = "";

        Thread Clantag;

        private void checkBox23_Checked(object sender, RoutedEventArgs e)
        {
            CurrentClantag = ReadString(xrpc_0(Convert.ToUInt32(listBox1.SelectedIndex)) + 0x55A0, 7);
            if (statuslabel2.Content == Client[0])
            {
                StartfunnyClantag(0);
            }
            if (statuslabel2.Content == Client[1])
            {
                StartfunnyClantag(1);
            }
            if (statuslabel2.Content == Client[2])
            {
                StartfunnyClantag(2); 
            }
            if (statuslabel2.Content == Client[3])
            {
                StartfunnyClantag(3); 
            }
            if (statuslabel2.Content == Client[4])
            {
                StartfunnyClantag(4);
            }
            if (statuslabel2.Content == Client[5])
            {
                StartfunnyClantag(5); 
            }
            if (statuslabel2.Content == Client[6])
            {
                StartfunnyClantag(6); 
            }
            if (statuslabel2.Content == Client[7])
            {
                StartfunnyClantag(7); 
            }
            if (statuslabel2.Content == Client[8])
            {
                StartfunnyClantag(8);
            }
            if (statuslabel2.Content == Client[9])
            {
                StartfunnyClantag(9);
            }
            if (statuslabel2.Content == Client[10])
            {
                StartfunnyClantag(10);
            }
            if (statuslabel2.Content == Client[11])
            {
                StartfunnyClantag(11);
            }
            if (statuslabel2.Content == Client[12])
            {
                StartfunnyClantag(12); 
            }
            if (statuslabel2.Content == Client[13])
            {
                StartfunnyClantag(13); 
            }
            if (statuslabel2.Content == Client[14])
            {
                StartfunnyClantag(14);
            }
            if (statuslabel2.Content == Client[15])
            {
                StartfunnyClantag(15); 
            }
            if (statuslabel2.Content == Client[16])
            {
                StartfunnyClantag(16);
            }
            if (statuslabel2.Content == Client[17])
            {
                StartfunnyClantag(17);
            }
        }

        private void checkBox23_Unchecked(object sender, RoutedEventArgs e)
        {
            WriteString(xrpc_0(Convert.ToUInt32(listBox1.SelectedIndex)) + 0x55A0, CurrentClantag);
            Clantag.Abort();
        }

        private void checkBox25_Checked(object sender, RoutedEventArgs e)
        {
            ColourGt(Convert.ToUInt32(listBox1.SelectedIndex), false);
        }

        private void checkBox25_Unchecked(object sender, RoutedEventArgs e)
        {
            ColourGt(Convert.ToUInt32(listBox1.SelectedIndex), true);
        }

        private void checkBox25_Checked_1(object sender, RoutedEventArgs e)
        {
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Fov ^2On\"");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "^ 5 \"90\"");
        }

        private void checkBox25_Unchecked_1(object sender, RoutedEventArgs e)
        {
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Fov ^1Off\"");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "^ 5 \"75\"");
        }

        private void checkBox26_Checked(object sender, RoutedEventArgs e)
        {
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Fov ^2On\"");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "^ 5 \"90\"");
        }

        private void checkBox26_Unchecked(object sender, RoutedEventArgs e)
        {
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Fov ^1Off\"");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "^ 5 \"75\"");
        }

        private void button2_Click_2(object sender, RoutedEventArgs e)
        {
            
        }

        private void label2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1.5));
            anim.Completed += anim_Completed;
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void headerThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Left = Left + e.HorizontalChange;
            Top = Top + e.VerticalChange;
        }

        private void label3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void button2_Click_3(object sender, RoutedEventArgs e)
        {
        }

        decimal velo1 = (decimal)0.0;

        void JetPack2(uint clientIndex)
        {
            for (; ; )
            {
                if (xrpc_2(clientIndex) == Buttons.A)
                {
                    if (velo1 < 100)
                    {
                        decimal velo2 = (decimal)15;
                        velo1 += velo2;
                        WriteFloat(xrpc_0(clientIndex) + 0x3c, (float)velo1);
                    }
                    else
                    {
                        decimal count = (decimal)15;
                        velo1 += count;
                        WriteFloat(xrpc_0(clientIndex) + 0x3c, (float)velo1);
                    }
                }
            }
        }

        Thread t2;
        Thread t3;

        public void StartJetpack(uint clientIndex)
        {
            t2 = new Thread(() => JetPack2(clientIndex));
            t2.Start();
        }


        private void button2_Click_4(object sender, RoutedEventArgs e)
        {
            
        }

        private void checkBox27_Checked(object sender, RoutedEventArgs e)
        {
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3JetPack given press A to use!\"");
            if (statuslabel2.Content == Client[0])
            {
                StartJetpack(0);
            }
            if (statuslabel2.Content == Client[1])
            {
                StartJetpack(1);
            }
            if (statuslabel2.Content == Client[2])
            {
                StartJetpack(2);
            }
            if (statuslabel2.Content == Client[3])
            {
                StartJetpack(3);
            }
            if (statuslabel2.Content == Client[4])
            {
                StartJetpack(4);
            }
            if (statuslabel2.Content == Client[5])
            {
                StartJetpack(5);
            }
            if (statuslabel2.Content == Client[6])
            {
                StartJetpack(6);
            }
            if (statuslabel2.Content == Client[7])
            {
                StartJetpack(7);
            }
            if (statuslabel2.Content == Client[8])
            {
                StartJetpack(8);
            }
            if (statuslabel2.Content == Client[9])
            {
                StartJetpack(9);
            }
            if (statuslabel2.Content == Client[10])
            {
                StartJetpack(10);
            }
            if (statuslabel2.Content == Client[11])
            {
                StartJetpack(11);
            }
            if (statuslabel2.Content == Client[12])
            {
                StartJetpack(12);
            }
            if (statuslabel2.Content == Client[13])
            {
                StartJetpack(13);
            }
            if (statuslabel2.Content == Client[14])
            {
                StartJetpack(14);
            }
            if (statuslabel2.Content == Client[15])
            {
                StartJetpack(15);
            }
            if (statuslabel2.Content == Client[16])
            {
                StartJetpack(16);
            }
            if (statuslabel2.Content == Client[17])
            {
                StartJetpack(17);
            }
            CHECK_25.IsEnabled = Check[1];
        }

        private void checkBox27_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void checkBox14_Checked(object sender, RoutedEventArgs e)
        {
            ThirdPerson(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3ThirdPerson ^2On\"");
        }

        private void checkBox14_Unchecked(object sender, RoutedEventArgs e)
        {
            ThirdPersonOff(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3ThirdPerson ^1Off\"");
        }

        private void checkBox19_Checked(object sender, RoutedEventArgs e)
        {
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3JetPack given press A to use!\"");
            if (statuslabel2.Content == Client[0])
            {
                StartJetpack(0);
            }
            if (statuslabel2.Content == Client[1])
            {
                StartJetpack(1);
            }
            if (statuslabel2.Content == Client[2])
            {
                StartJetpack(2);
            }
            if (statuslabel2.Content == Client[3])
            {
                StartJetpack(3);
            }
            if (statuslabel2.Content == Client[4])
            {
                StartJetpack(4);
            }
            if (statuslabel2.Content == Client[5])
            {
                StartJetpack(5);
            }
            if (statuslabel2.Content == Client[6])
            {
                StartJetpack(6);
            }
            if (statuslabel2.Content == Client[7])
            {
                StartJetpack(7);
            }
            if (statuslabel2.Content == Client[8])
            {
                StartJetpack(8);
            }
            if (statuslabel2.Content == Client[9])
            {
                StartJetpack(9);
            }
            if (statuslabel2.Content == Client[10])
            {
                StartJetpack(10);
            }
            if (statuslabel2.Content == Client[11])
            {
                StartJetpack(11);
            }
            if (statuslabel2.Content == Client[12])
            {
                StartJetpack(12);
            }
            if (statuslabel2.Content == Client[13])
            {
                StartJetpack(13);
            }
            if (statuslabel2.Content == Client[14])
            {
                StartJetpack(14);
            }
            if (statuslabel2.Content == Client[15])
            {
                StartJetpack(15);
            }
            if (statuslabel2.Content == Client[16])
            {
                StartJetpack(16);
            }
            if (statuslabel2.Content == Client[17])
            {
                StartJetpack(17);
            }
            CHECK_6.IsEnabled = Check[1];
        }

        private void checkBox17_Checked(object sender, RoutedEventArgs e)
        {
            DefaultWeapon_zmp = 1;
            Minigun_zm = 0x43;
            Raygun_zm = 0x44;
            Fuckyouiamwizard_zm = 0x45;
            xrpc_1(Offsets.SENDSERVERCOMMAND, -1, 0, "; \"^3Weapon ID alternated!\"");
        }

        private void checkBox17_Unchecked(object sender, RoutedEventArgs e)
        {
            DefaultWeapon_zmp = 1;
            Minigun_zm = 0x33;
            Raygun_zm = 0x31;
            Fuckyouiamwizard_zm = 0x54;
            xrpc_1(Offsets.SENDSERVERCOMMAND, -1, 0, "; \"^3Weapon ID alternated!\"");
        }

        private void WEAP_ZM_3_Checked_1(object sender, RoutedEventArgs e)
        {
            GiveWeapon_ZM(Convert.ToUInt32(listBox1.SelectedIndex), "FUCKYOUIAMWIZARD_ZM");
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Weapon Given!\"");
            WEAP_ZM_1.IsChecked = null;
            WEAP_ZM_2.IsChecked = null;
            WEAP_ZM_0.IsChecked = null;
        }

        private void WEAP_ZM_0_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void checkBox28_Checked(object sender, RoutedEventArgs e)
        {
            GivePerks(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Perks Given!\"");
        }

        private void checkBox29_Unchecked(object sender, RoutedEventArgs e)
        {
            GiveDoublePoints(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Double Points Given!\"");
        }

        private void checkBox29_Checked(object sender, RoutedEventArgs e)
        {
            GiveDoublePoints(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Double Points Given!\"");
        }

        private void checkBox28_Unchecked(object sender, RoutedEventArgs e)
        {
            TakePerks(Convert.ToUInt32(listBox1.SelectedIndex));
            xrpc_1(Offsets.SENDSERVERCOMMAND, listBox1.SelectedIndex, 0, "; \"^3Perks Taken!\"");
        }
    }
}
