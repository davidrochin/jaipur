using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreadorDeSets : MonoBehaviour {

    [Header("Ajustes")]
    public TipoSet tipo = TipoSet.Cartas;
    public GameObject prefabCarta;
    public GameObject prefabFicha;

    Carta.TipoMercancia[] mazoDefecto = {
        Carta.TipoMercancia.Cuero, Carta.TipoMercancia.Cuero, Carta.TipoMercancia.Cuero, Carta.TipoMercancia.Cuero, Carta.TipoMercancia.Cuero, Carta.TipoMercancia.Cuero,
        Carta.TipoMercancia.Cuero, Carta.TipoMercancia.Cuero, Carta.TipoMercancia.Cuero, Carta.TipoMercancia.Cuero, Carta.TipoMercancia.Especias, Carta.TipoMercancia.Especias,
        Carta.TipoMercancia.Especias, Carta.TipoMercancia.Especias, Carta.TipoMercancia.Especias, Carta.TipoMercancia.Especias, Carta.TipoMercancia.Especias,
        Carta.TipoMercancia.Especias,
        Carta.TipoMercancia.Tela, Carta.TipoMercancia.Tela, Carta.TipoMercancia.Tela, Carta.TipoMercancia.Tela, Carta.TipoMercancia.Tela, Carta.TipoMercancia.Tela,
        Carta.TipoMercancia.Tela, Carta.TipoMercancia.Tela,
        Carta.TipoMercancia.Plata, Carta.TipoMercancia.Plata, Carta.TipoMercancia.Plata, Carta.TipoMercancia.Plata, Carta.TipoMercancia.Plata, Carta.TipoMercancia.Plata,
        Carta.TipoMercancia.Oro, Carta.TipoMercancia.Oro, Carta.TipoMercancia.Oro, Carta.TipoMercancia.Oro, Carta.TipoMercancia.Oro, Carta.TipoMercancia.Oro,
        Carta.TipoMercancia.Diamante, Carta.TipoMercancia.Diamante, Carta.TipoMercancia.Diamante, Carta.TipoMercancia.Diamante, Carta.TipoMercancia.Diamante, Carta.TipoMercancia.Diamante,
        Carta.TipoMercancia.Camello, Carta.TipoMercancia.Camello, Carta.TipoMercancia.Camello, Carta.TipoMercancia.Camello, Carta.TipoMercancia.Camello, Carta.TipoMercancia.Camello,
        Carta.TipoMercancia.Camello, Carta.TipoMercancia.Camello, Carta.TipoMercancia.Camello, Carta.TipoMercancia.Camello, Carta.TipoMercancia.Camello
    };

    InfoFicha[] mazoDefectoFichas;

	void Awake () {

        //Inicializar el set por defecto de Fichas
        InicializarFichasDefecto();

        CrearSet();

	}

    public void CrearSet() {
        if (tipo == TipoSet.Cartas) {
            //Agregar el mazo de cartas por defecto
            for (int x = 0; x < 55; x++) {
                Carta carta; GameObject cartaObjeto;
                cartaObjeto = Instantiate(prefabCarta, Vector3.zero, Quaternion.identity);
                cartaObjeto.transform.SetParent(transform);
                carta = cartaObjeto.GetComponent<Carta>();

                carta.SetMercancia(mazoDefecto[x]);
                carta.id = x;
                carta.gameObject.name = "carta_" + carta.mercancia.ToString().ToLower();
            }
        } else if (tipo == TipoSet.Fichas) {
            //Agregar el mazo de fichas por defecto
            for (int x = 0; x < 38; x++) {
                Ficha ficha; GameObject fichaObjecto;
                fichaObjecto = Instantiate(prefabFicha, Vector3.zero, Quaternion.identity);
                fichaObjecto.transform.SetParent(transform);
                fichaObjecto.transform.SetAsLastSibling();
                ficha = fichaObjecto.GetComponent<Ficha>();

                ficha.valorFicha = mazoDefectoFichas[x].valor;
                ficha.tipoFicha = mazoDefectoFichas[x].tipo;

                ficha.ActualizarApariencia();
            }
        }
    }

    public static void BorrarTodasLasCartasYFichas() {
        foreach (Carta carta in FindObjectsOfType<Carta>()) {
            Destroy(carta.gameObject);
        }
        foreach (Ficha ficha in FindObjectsOfType<Ficha>()) {
            Destroy(ficha.gameObject);
        }
    }

    void InicializarFichasDefecto() {

        mazoDefectoFichas = new InfoFicha[38];

        //Cuero 9
        mazoDefectoFichas[0] = new InfoFicha(Ficha.TipoFicha.Cuero, 1);
        mazoDefectoFichas[1] = new InfoFicha(Ficha.TipoFicha.Cuero, 1);
        mazoDefectoFichas[2] = new InfoFicha(Ficha.TipoFicha.Cuero, 1);
        mazoDefectoFichas[3] = new InfoFicha(Ficha.TipoFicha.Cuero, 1);
        mazoDefectoFichas[4] = new InfoFicha(Ficha.TipoFicha.Cuero, 1);
        mazoDefectoFichas[5] = new InfoFicha(Ficha.TipoFicha.Cuero, 1);
        mazoDefectoFichas[6] = new InfoFicha(Ficha.TipoFicha.Cuero, 2);
        mazoDefectoFichas[7] = new InfoFicha(Ficha.TipoFicha.Cuero, 3);
        mazoDefectoFichas[8] = new InfoFicha(Ficha.TipoFicha.Cuero, 4);

        //Especias 7
        mazoDefectoFichas[9] = new InfoFicha(Ficha.TipoFicha.Especias, 1);
        mazoDefectoFichas[10] = new InfoFicha(Ficha.TipoFicha.Especias, 1);
        mazoDefectoFichas[11] = new InfoFicha(Ficha.TipoFicha.Especias, 2);
        mazoDefectoFichas[12] = new InfoFicha(Ficha.TipoFicha.Especias, 2);
        mazoDefectoFichas[13] = new InfoFicha(Ficha.TipoFicha.Especias, 3);
        mazoDefectoFichas[14] = new InfoFicha(Ficha.TipoFicha.Especias, 3);
        mazoDefectoFichas[15] = new InfoFicha(Ficha.TipoFicha.Especias, 5);

        //Tela 7
        mazoDefectoFichas[16] = new InfoFicha(Ficha.TipoFicha.Tela, 1);
        mazoDefectoFichas[17] = new InfoFicha(Ficha.TipoFicha.Tela, 1);
        mazoDefectoFichas[18] = new InfoFicha(Ficha.TipoFicha.Tela, 2);
        mazoDefectoFichas[19] = new InfoFicha(Ficha.TipoFicha.Tela, 2);
        mazoDefectoFichas[20] = new InfoFicha(Ficha.TipoFicha.Tela, 3);
        mazoDefectoFichas[21] = new InfoFicha(Ficha.TipoFicha.Tela, 3);
        mazoDefectoFichas[22] = new InfoFicha(Ficha.TipoFicha.Tela, 5);

        //Plata 5
        mazoDefectoFichas[23] = new InfoFicha(Ficha.TipoFicha.Plata, 5);
        mazoDefectoFichas[24] = new InfoFicha(Ficha.TipoFicha.Plata, 5);
        mazoDefectoFichas[25] = new InfoFicha(Ficha.TipoFicha.Plata, 5);
        mazoDefectoFichas[26] = new InfoFicha(Ficha.TipoFicha.Plata, 5);
        mazoDefectoFichas[27] = new InfoFicha(Ficha.TipoFicha.Plata, 5);

        //Oro 5
        mazoDefectoFichas[28] = new InfoFicha(Ficha.TipoFicha.Oro, 5);
        mazoDefectoFichas[29] = new InfoFicha(Ficha.TipoFicha.Oro, 5);
        mazoDefectoFichas[30] = new InfoFicha(Ficha.TipoFicha.Oro, 5);
        mazoDefectoFichas[31] = new InfoFicha(Ficha.TipoFicha.Oro, 6);
        mazoDefectoFichas[32] = new InfoFicha(Ficha.TipoFicha.Oro, 6);

        //Diamante 5
        mazoDefectoFichas[33] = new InfoFicha(Ficha.TipoFicha.Diamante, 5);
        mazoDefectoFichas[34] = new InfoFicha(Ficha.TipoFicha.Diamante, 5);
        mazoDefectoFichas[35] = new InfoFicha(Ficha.TipoFicha.Diamante, 5);
        mazoDefectoFichas[36] = new InfoFicha(Ficha.TipoFicha.Diamante, 7);
        mazoDefectoFichas[37] = new InfoFicha(Ficha.TipoFicha.Diamante, 7);
    }

    public enum TipoSet { Cartas, Fichas }
}

public class InfoFicha {
    public Ficha.TipoFicha tipo;
    public int valor;

    public InfoFicha(Ficha.TipoFicha tipo, int valor) {
        this.tipo = tipo;
        this.valor = valor;
    }
}
