using Futebol.comum;
using Futebol.log;
using Google.Protobuf;
using RefreeProto;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Futebol.src.juiz
{
    public class SubstitutoCliente : ClienteBase
    {
        private Color _corDoTime;
        private LogUI _log;
        private List<Tuple<Vector2D, double>> _robos;
        private Mutex _mutex;

        public SubstitutoCliente(string endIp, int porta, LogUI log) : base(endIp, porta)
        {
            _log = log;
            _mutex = new Mutex();
            _robos = new List<Tuple<Vector2D, double>>();
        }
        public void SetCorDoTime(Color cor)
        {
            _corDoTime = cor;
        }
        public void PosicionarRobo(int idRobo, Vector2D posicoes, double angulo)
        {
            _mutex.WaitOne();
            _robos[idRobo] = new Tuple<Vector2D, double>(posicoes, angulo);
            _mutex.ReleaseMutex();
        }
        public void EnviarFrame()
        {
            if (!conectado)
            {
                Iniciar();
            }
            VSSRef_Placement posicionamentos = new VSSRef_Placement();
            Frame frame = new Frame();
            frame.TeamColor = _corDoTime;
            _mutex.WaitOne();
            for (int i = 0; i < _robos.Count; i++)
            {
                Tuple<Vector2D, double> robo = _robos[i];
                Robot robot = new Robot();
                robot.RobotId = (uint)i;
                robot.X = robo.Item1.x;
                robot.Y = robo.Item1.y;
                robot.Orientation = robo.Item2;
                frame.Robots.Add(robot);
            }
            _mutex.ReleaseMutex();
            posicionamentos.World = frame;
            byte[] buffer = posicionamentos.ToByteArray();
            try
            {
                udpClient.Send(buffer);
            }
            catch (Exception exception)
            {
                _log.InserirMensagem($"Erro ao tentar enviar frame via socket\n{exception}");
            }
            _robos.Clear();
        }
        protected override void ConectarRede()
        {
            try
            {
                udpClient = new UdpClient(ipEndPoint);
            }
            catch (Exception exeption)
            {
                _log.InserirMensagem($"Ocorreu um erro ao tentar conectar com o juíz!\n{exeption}");
            }
        }
        protected override void DesconectarRede()
        {
            try
            {
                if (udpClient.Client.Connected)
                {
                    udpClient.Client.Close();
                    udpClient.Dispose();
                }
            }
            catch (Exception exception)
            {
                _log.InserirMensagem($"Erro ao tentar desconectar juiz!\n{exception}");
            }
        }
        protected override void IniciarCliente()
        {

        }
    }
}
