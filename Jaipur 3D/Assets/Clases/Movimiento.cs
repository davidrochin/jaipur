using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Movimiento {
    public int[] ids;
    public TipoMovimiento tipoMovimiento;

    public enum TipoMovimiento { Ninguno, Tomar, Vender, Trueque, OrdenMazoPrincipal }

    public Movimiento(TipoMovimiento tipoMov, int[] cartas) {
        tipoMovimiento = tipoMov;
        ids = cartas;
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
