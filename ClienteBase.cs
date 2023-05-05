using System.Net;
using System.Net.Sockets;

namespace Futebol.src.juiz
{
    public abstract class ClienteBase
    {
        protected bool conectado;
        protected IPEndPoint ipEndPoint;
        protected UdpClient udpClient;
        public ClienteBase(string endIp, int porta)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(endIp), porta);
            conectado = false;
        }
        protected virtual void ConectarRede() { }
        protected virtual void DesconectarRede() { }
        protected virtual void IniciarCliente() { }
        public void Iniciar()
        {
            if (!conectado)
            {
                ConectarRede();
                conectado = true;
            }
            IniciarCliente();
        }
        public void Parar()
        {
            if (conectado)
            {
                DesconectarRede();
                conectado = false;
            }
        }
    }
}
