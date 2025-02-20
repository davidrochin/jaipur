﻿using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;

public class Movimiento : MessageBase {
    public static short TIPO = 100;

    public int[] ids;
    public TipoMovimiento tipoMovimiento;

    public enum TipoMovimiento { Ninguno, Tomar, Vender, Trueque, OrdenMazoPrincipal }

    public override string ToString() {
        string cadena = "" + tipoMovimiento;
        foreach (int id in ids) {
            cadena = cadena + ", " + id;
        }
        return cadena;
    }

    public bool SeEncuentraId(int id) {
        foreach (int actual in ids) {
            if(actual == id) {
                return true;
            }
        }
        return false;
    }
}

public class MensajeOrden : MessageBase {
    public static short TIPO = 101;
    public int[] ids;
    public string nombreGrupo;
    public Grupo.TipoGrupo tipoGrupo;

    public override string ToString() {
        string cadena = "" + tipoGrupo;
        foreach (int id in ids) {
            cadena = cadena + ", " + id;
        }
        return cadena;
    }

    public bool SeEncuentraId(int id) {
        foreach (int actual in ids) {
            if (actual == id) {
                return true;
            }
        }
        return false;
    }
}

public class MensajeString : MessageBase {
    public static short TIPO = 102;
    public string mensaje;
}

public class MensajeAccion : MessageBase {
    public static short TIPO = 103;
    public TipoAccion tipoAccion;
    public enum TipoAccion { IniciarJuego, IniciarRonda, AcabarRonda, AcabarJuego, Salir }
}

public class MensajeTurno : MessageBase {
    public static short TIPO = 104;
    public Jugador jugador;
    public enum Jugador { Anfitrion, Invitado }
}