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
            for (int x = 0; x < mazoDefectoFichas.Length; x++) {
                Ficha ficha; GameObject fichaObjecto;
                fichaObjecto = Instantiate(prefabFicha, Vector3.zero, Quaternion.identity);
                fichaObjecto.transform.SetParent(transform);
                fichaObjecto.transform.SetAsLastSibling();
                ficha = fichaObjecto.GetComponent<Ficha>();

                ficha.valorFicha = mazoDefectoFichas[x].valor;
                ficha.tipoFicha = mazoDefectoFichas[x].tipo;
                ficha.id = x;
                ficha.name = "ficha_" + ficha.tipoFicha.ToString().ToLower();

                ficha.ActualizarApariencia();
            }
        }
    }

    void InicializarFichasDefecto() {
        mazoDefectoFichas = new InfoFicha[] {
            new InfoFicha(Ficha.TipoFicha.Cuero, 1),
            new InfoFicha(Ficha.TipoFicha.Cuero, 1),
            new InfoFicha(Ficha.TipoFicha.Cuero, 1),
            new InfoFicha(Ficha.TipoFicha.Cuero, 1),
            new InfoFicha(Ficha.TipoFicha.Cuero, 1),
            new InfoFicha(Ficha.TipoFicha.Cuero, 1),
            new InfoFicha(Ficha.TipoFicha.Cuero, 2),
            new InfoFicha(Ficha.TipoFicha.Cuero, 3),
            new InfoFicha(Ficha.TipoFicha.Cuero, 4),
            new InfoFicha(Ficha.TipoFicha.Especias, 1),
            new InfoFicha(Ficha.TipoFicha.Especias, 1),
            new InfoFicha(Ficha.TipoFicha.Especias, 2),
            new InfoFicha(Ficha.TipoFicha.Especias, 2),
            new InfoFicha(Ficha.TipoFicha.Especias, 3),
            new InfoFicha(Ficha.TipoFicha.Especias, 3),
            new InfoFicha(Ficha.TipoFicha.Especias, 5),
            new InfoFicha(Ficha.TipoFicha.Tela, 1),
            new InfoFicha(Ficha.TipoFicha.Tela, 1),
            new InfoFicha(Ficha.TipoFicha.Tela, 2),
            new InfoFicha(Ficha.TipoFicha.Tela, 2),
            new InfoFicha(Ficha.TipoFicha.Tela, 3),
            new InfoFicha(Ficha.TipoFicha.Tela, 3),
            new InfoFicha(Ficha.TipoFicha.Tela, 5),
            new InfoFicha(Ficha.TipoFicha.Plata, 5),
            new InfoFicha(Ficha.TipoFicha.Plata, 5),
            new InfoFicha(Ficha.TipoFicha.Plata, 5),
            new InfoFicha(Ficha.TipoFicha.Plata, 5),
            new InfoFicha(Ficha.TipoFicha.Plata, 5),
            new InfoFicha(Ficha.TipoFicha.Oro, 5),
            new InfoFicha(Ficha.TipoFicha.Oro, 5),
            new InfoFicha(Ficha.TipoFicha.Oro, 5),
            new InfoFicha(Ficha.TipoFicha.Oro, 6),
            new InfoFicha(Ficha.TipoFicha.Oro, 6),
            new InfoFicha(Ficha.TipoFicha.Diamante, 5),
            new InfoFicha(Ficha.TipoFicha.Diamante, 5),
            new InfoFicha(Ficha.TipoFicha.Diamante, 5),
            new InfoFicha(Ficha.TipoFicha.Diamante, 7),
            new InfoFicha(Ficha.TipoFicha.Diamante, 7),
            new InfoFicha(Ficha.TipoFicha.Camello, 5),
            new InfoFicha(Ficha.TipoFicha.Bonus3, 1),
            new InfoFicha(Ficha.TipoFicha.Bonus3, 1),
            new InfoFicha(Ficha.TipoFicha.Bonus3, 2),
            new InfoFicha(Ficha.TipoFicha.Bonus3, 2),
            new InfoFicha(Ficha.TipoFicha.Bonus3, 2),
            new InfoFicha(Ficha.TipoFicha.Bonus3, 3),
            new InfoFicha(Ficha.TipoFicha.Bonus3, 3),
            new InfoFicha(Ficha.TipoFicha.Bonus4, 4),
            new InfoFicha(Ficha.TipoFicha.Bonus4, 4),
            new InfoFicha(Ficha.TipoFicha.Bonus4, 5),
            new InfoFicha(Ficha.TipoFicha.Bonus4, 5),
            new InfoFicha(Ficha.TipoFicha.Bonus4, 6),
            new InfoFicha(Ficha.TipoFicha.Bonus4, 6),
            new InfoFicha(Ficha.TipoFicha.Bonus5, 8),
            new InfoFicha(Ficha.TipoFicha.Bonus5, 8),
            new InfoFicha(Ficha.TipoFicha.Bonus5, 9),
            new InfoFicha(Ficha.TipoFicha.Bonus5, 10),
            new InfoFicha(Ficha.TipoFicha.Bonus5, 10)};
    }

    public static void BorrarTodasLasCartasYFichas() {
        foreach (Carta carta in FindObjectsOfType<Carta>()) {
            Destroy(carta.gameObject);
        }
        foreach (Ficha ficha in FindObjectsOfType<Ficha>()) {
            Destroy(ficha.gameObject);
        }
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
