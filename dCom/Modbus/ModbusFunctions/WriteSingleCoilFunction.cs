using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
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

            ushort regAddress = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(response, 8));
            ushort vrednost = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(response, 10));
            recnik.Add(Tuple.Create(PointType.DIGITAL_OUTPUT, regAddress), vrednost);

            return recnik;
        }
    }
}