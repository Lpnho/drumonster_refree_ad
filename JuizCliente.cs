using Futebol.log;
using Google.Protobuf;
using RefreeProto;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Futebol.src.juiz
{
    public class JuizCliente : ClienteBase
    {
        private Tuple<Foul, Color, Quadrant> _dadosUltimaFalta;
        private LogUI _log;
        private Mutex _mutex;
        public JuizCliente(string endIp, int porta, LogUI log) : base(endIp, porta)
        {
            _log = log;
            _mutex = new Mutex();
        }
        public Foul UltimaFalta()
        {
            _mutex.WaitOne();
            Foul falta = _dadosUltimaFalta.Item1;
            _mutex.ReleaseMutex();
            return falta;
        }
        public Color CorUltimaFalta() 
        {
            _mutex.WaitOne();
            Color cor = _dadosUltimaFalta.Item2;
            _mutex.ReleaseMutex();
            return cor;
        }
        public Quadrant QuadranteUltimaFalta()
        {
            _mutex.WaitOne();
            Quadrant quadrante = _dadosUltimaFalta.Item3;
            _mutex.ReleaseMutex();
            return quadrante;
        }
        protected override void ConectarRede()
        {
            try
            {
                udpClient = new UdpClient(ipEndPoint);
                udpClient.ExclusiveAddressUse = false;
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
                udpClient.JoinMulticastGroup(ipEndPoint.Address);
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
            while (true)
            {
                if (udpClient.Available > 0)
                {
                    VSSRef_Command comando = new VSSRef_Command();
                    try
                    {
                        byte[] datagramas = udpClient.Receive(ref ipEndPoint);
                        comando.MergeFrom(datagramas);
                        _mutex.WaitOne();
                        _dadosUltimaFalta = new Tuple<Foul, Color, Quadrant>(comando.Foul, comando.Teamcolor, comando.FoulQuadrant);
                        _mutex.ReleaseMutex();
                    }
                    catch (Exception exeption)
                    {
                        _log.InserirMensagem($"Erro ao receber mensagem do juiz!\n{exeption}");
                    }
                }
            }
        }

    }
}
