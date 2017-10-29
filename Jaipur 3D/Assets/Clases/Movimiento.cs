using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Movimiento {
    public int[] ids;
    public TipoMovimiento tipoMovimiento;

    public enum TipoMovimiento { Ninguno, Tomar, Vender, Trueque, RevolverCartas }

    public Movimiento(TipoMovimiento tipoMov) {
        tipoMovimiento = tipoMov;
    }

    public byte[] Serializar() {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(stream, this);
        return stream.GetBuffer();
    }

    public static Movimiento Deserializar(byte[] datos) {
        MemoryStream stream = new MemoryStream(datos);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        return (Movimiento)binaryFormatter.Deserialize(stream);
    }
}
