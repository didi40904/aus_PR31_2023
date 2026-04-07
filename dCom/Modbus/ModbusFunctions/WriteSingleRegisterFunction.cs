using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write single register functions/requests.
    /// </summary>
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleRegisterFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()     
        {

            ModbusWriteCommandParameters zaPisanje = (ModbusWriteCommandParameters)CommandParameters;
            byte[] niz = new byte[12];

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaPisanje.TransactionId)), 0, niz, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaPisanje.ProtocolId)), 0, niz, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaPisanje.Length)), 0, niz, 4, 2);
            niz[6] = zaPisanje.UnitId;
            niz[7] = zaPisanje.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaPisanje.OutputAddress)), 0, niz, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)zaPisanje.Value)), 0, niz, 10, 2);

            return niz;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)  
        {
            ModbusWriteCommandParameters zaPisanje = (ModbusWriteCommandParameters)CommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> recnik = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ushort regAddress = (ushort)((response[8] << 8) | response[9]); 

            ushort vrednost = (ushort)((response[10] << 8) | response[11]);  

            recnik.Add(Tuple.Create(PointType.ANALOG_OUTPUT, (ushort)regAddress), (ushort)vrednost);

            return recnik;
        }
    }
}