using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManejadorJuego : MonoBehaviour {

    [Header("Partida")]
    public bool turnoJugador = true;
    public int ronda = 1;

    [Header("Grupos")]
    public Grupo mazoMercado;
    public Grupo mazoPrincipal, mazoDescartar, mazoJugador, mazoJugadorCamellos, mazoOponente, mazoOponenteCamellos;

    [Header("Fichas")]
    public Grupo fichasPrincipal;
    public Grupo fichasDiamante, fichasOro, fichasPlata, fichasTela, fichasEspecias, fichasCuero;

    [Header("Interfaz")]
    public GameObject panelAcciones;

    [Header("Selección")]
    public List<GameObject> objetosSeleccionados;
    List<Carta> cartasSeleccionadas;

    //Esta variable es la que prevee que el juego siga avanzando si se está ejecutando alguna animación, o algo por el estilo.
    private bool ocupado = false;

    private const float TIEMPO_ENTRE_CARTA_ANIMADA = 0.3f;

    void Awake() {
        //Inicializar el List
        objetosSeleccionados = new List<GameObject>();
        cartasSeleccionadas = new List<Carta>();

        //Inicializar elementos de la interfaz
        if (panelAcciones == null) { panelAcciones = GameObject.Find("panel_acciones"); }
        EstadoPanelAcciones(false);

        //Obtener el cliete y servidor
        cliente = FindObjectOfType<Cliente>();
        servidor = FindObjectOfType<Servidor>();
    }

    void Start() {
        //Iniciar el juego
        StartCoroutine(Juego());
    }

    void Update() {

    }

    #region Inicial

    public void AgregarCamellosIniciales() {
        foreach (GameObject camello in mazoPrincipal.ObtenerCamellos(3)) {
            camello.transform.SetParent(mazoMercado.transform);
        }
    }

    public void LlenarMercado() {
        int necesarias = 5 - mazoMercado.hijos.Length;
        for (int x = 0; x < necesarias; x++) {
            GameObject carta = mazoPrincipal.ObtenerUltimaCarta();
            if (carta != null) { carta.transform.SetParent(mazoMercado.gameObject.transform); }

        }
    }

    public void RepartirAJugadores() {

        //Repartir al jugador
        foreach (GameObject x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            DarAJugador(x);
        }

        //Repartir al oponente
        foreach (GameObject x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            DarAOponente(x);
        }

    }

    public void OrdenarFichas() {

        List<Transform> temp = new List<Transform>();

        foreach (Transform hijo in fichasPrincipal.transform) {
            temp.Add(hijo);
        }

        foreach (Transform hijo in temp) {

            Ficha ficha = hijo.GetComponent<Ficha>();

            switch (ficha.tipoFicha) {
                case Ficha.TipoFicha.Diamante:
                    ficha.transform.SetParent(fichasDiamante.transform);
                    break;
                case Ficha.TipoFicha.Oro:
                    ficha.transform.SetParent(fichasOro.transform);
                    break;
                case Ficha.TipoFicha.Plata:
                    ficha.transform.SetParent(fichasPlata.transform);
                    break;
                case Ficha.TipoFicha.Tela:
                    ficha.transform.SetParent(fichasTela.transform);
                    break;
                case Ficha.TipoFicha.Especias:
                    ficha.transform.SetParent(fichasEspecias.transform);
                    break;
                case Ficha.TipoFicha.Cuero:
                    ficha.transform.SetParent(fichasCuero.transform);
                    break;
            }
        }
    }

    public void DarAJugador(GameObject carta) {
        if (carta.GetComponent<Carta>().mercancia != Carta.TipoMercancia.Camello) {
            carta.transform.SetParent(mazoJugador.transform);
            mazoJugador.OrdenarPorTipo();
        } else {
            carta.transform.SetParent(mazoJugadorCamellos.transform);
        }
    }

    public void DarAOponente(GameObject carta) {
        if (carta.GetComponent<Carta>().mercancia != Carta.TipoMercancia.Camello) {
            carta.transform.SetParent(mazoOponente.transform);
        } else {
            carta.transform.SetParent(mazoOponenteCamellos.transform);
        }
    }

    #endregion

    #region Selección

    public void ChecarSeleccion() {
        //Checar si se seleccionó un camello del mercado
        /*foreach (Carta carta in cartasSeleccionadas) {
            Debug.Log(carta.tipoMaterial);
            if(carta.mazo == mazoMercado && carta.tipoMaterial == Carta.MaterialCarta.Camello) {
                Debug.Log("Limpiando seleccion");
                LimpiarSeleccion();

                //Seleccionar a todos los camellos del mercado
                foreach (Carta x in mazoMercado.hijosCarta) {
                    if (x.tipoMaterial == Carta.MaterialCarta.Camello) {
                        Debug.Log("Agregando carta");
                        AgregarASeleccion(x.gameObject, false);
                        
                    }
                }
                break;
            }
        }*/
    }

    public bool AgregarASeleccion(GameObject objeto, bool checarSeleccion) {
        //Revisar si ya se seleccionaron las 10 cartas
        if (objetosSeleccionados.Count < 10 && ocupado == false) {
            Carta carta = objeto.GetComponent<Carta>();

            //Revisar si se seleccionó algun camello del mercado
            if (carta.mercancia == Carta.TipoMercancia.Camello && carta.grupo == mazoMercado && !seleccionadosTodosCamellosMercado) {
                LimpiarSeleccion(mazoMercado);
                SeleccionarCamellosMercado(true);
                return true;
            } else if (seleccionadosTodosCamellosMercado && carta.mercancia != Carta.TipoMercancia.Camello && carta.grupo == mazoMercado) {
                SeleccionarCamellosMercado(false);
            }

            //Añadir la carta a la selección
            objetosSeleccionados.Add(objeto);
            carta.SetSeleccionada(true);
            ActualizarReferenciasCartas();

            if (checarSeleccion) { ChecarSeleccion(); }

            return true;
        } else {
            //No añadirla y regresar false
            return false;
        }
    }

    public bool RemoverDeSeleccion(GameObject objeto, bool checarSeleccion) {

        Carta carta = objeto.GetComponent<Carta>();

        if (seleccionadosTodosCamellosMercado && carta.mercancia == Carta.TipoMercancia.Camello && carta.grupo == mazoMercado) {
            SeleccionarCamellosMercado(false);
            return true;
        }

        foreach (GameObject x in objetosSeleccionados) {
            if (objeto == x) {
                objetosSeleccionados.Remove(x);
                carta.SetSeleccionada(false);
                ActualizarReferenciasCartas();

                if (checarSeleccion) { ChecarSeleccion(); }

                return true;
            }
        }
        return false;
    }

    public void LimpiarSeleccion() {
        foreach (Carta carta in cartasSeleccionadas) {
            carta.SetSeleccionada(false);
        }
        objetosSeleccionados.Clear();
        ActualizarReferenciasCartas();
        seleccionadosTodosCamellosMercado = false;
    }

    public void LimpiarSeleccion(Grupo mazo) {
        foreach (Carta carta in cartasSeleccionadas) {
            if (carta.grupo == mazo) {
                carta.SetSeleccionada(false);
            }
        }
        objetosSeleccionados.Clear();
        ActualizarReferenciasCartas();
    }

    public void ActualizarReferenciasCartas() {
        cartasSeleccionadas.Clear();

        foreach (GameObject x in objetosSeleccionados) {
            cartasSeleccionadas.Add(x.GetComponent<Carta>());
        }
    }

    bool seleccionadosTodosCamellosMercado = false;

    public void SeleccionarCamellosMercado(bool seleccion) {
        foreach (Transform hijo in mazoMercado.transform) {

            Carta carta = hijo.GetComponent<Carta>();

            if (carta.mercancia == Carta.TipoMercancia.Camello) {

                if (seleccion == true) {
                    objetosSeleccionados.Add(carta.gameObject);
                    carta.SetSeleccionada(true);
                    seleccionadosTodosCamellosMercado = true;
                } else {
                    objetosSeleccionados.Remove(carta.gameObject);
                    carta.SetSeleccionada(false);
                    seleccionadosTodosCamellosMercado = false;
                }

                ActualizarReferenciasCartas();
            }
        }
    }

    #endregion

    #region Acciones del Jugador

    public void TomarUna() {

        //Revisar si son solo camellos
        bool sonSoloCamellos = true;
        foreach (Carta carta in cartasSeleccionadas) {
            if (carta.mercancia != Carta.TipoMercancia.Camello) {
                sonSoloCamellos = false;
            }
        }

        //Revisar que solo se haya seleccionado 1 carta si no son solo camellos
        if (sonSoloCamellos == false) {
            if (cartasSeleccionadas.Count < 1) {
                Debug.Log("Necesitas seleccionar al menos una carta.");
                return;
            } else if (cartasSeleccionadas.Count > 1) {
                Debug.Log("No puedes tomar mas de una carta, al menos sean camellos.");
                return;
            }
        }

        Debug.Log(mazoMercado);
        //Revisar si la carta es del mercado
        if (cartasSeleccionadas[0].grupo != mazoMercado) {
            Debug.Log("Solo puedes tomar cartas del mercado.");
            return;
        }

        //Revisar que el jugador pueda aceptar las cartas
        int cantidadCartas = 0; int cantidadCamellos = 0;
        foreach (Carta carta in cartasSeleccionadas) {
            if (carta.mercancia != Carta.TipoMercancia.Camello) {
                cantidadCartas++;
            } else {
                cantidadCamellos++;
            }
        }
        if (PuedeAceptar(cantidadCartas, cantidadCamellos) == false) {
            Debug.Log("Solo puedes tener 7 cartas normales y 7 cartas de camello.");
            return;
        }

        //Todo está bien
        foreach (GameObject objeto in objetosSeleccionados) {
            cartasMovidas.Add(objeto.GetComponent<Carta>().id);
            DarAJugador(objeto);
        }

        //Generar el objeto tipo Movimiento que se va a enviar
        //movimiento = new Movimiento(Movimiento.TipoMovimiento.Tomar, cartasMovidas.ToArray());

        LimpiarSeleccion();
        LlenarMercado();
    }

    public void Vender() {

        //Revisar que haya cartas seleccionadas
        if (cartasSeleccionadas.Count < 1) {
            Debug.Log("¡Necesitas seleccionar al menos una carta!");
            return;
        }

        Carta.TipoMercancia primerMaterial = cartasSeleccionadas[0].mercancia;
        foreach (Carta carta in cartasSeleccionadas) {

            //Revisar que las cartas no sean del mercado
            if (carta.grupo == mazoMercado) {
                Debug.Log("¡No puedes vender cartas del mercado!");
                return;
            }

            //Revisar que no sean camellos
            if (carta.mercancia == Carta.TipoMercancia.Camello) {
                Debug.Log("¡No puedes vender camellos!");
                return;
            }

            //Revisar que sean del mismo material
            if (carta.mercancia != primerMaterial) {
                Debug.Log("¡No puedes vender cartas de diferente tipo de material!");
                return;
            }
        }

        //Todo en orden. Vender las cartas
        foreach (Carta carta in cartasSeleccionadas) {
            cartasMovidas.Add(carta.id);
            carta.transform.SetParent(mazoDescartar.transform);
        }

        //Generar el objeto de tipo Movimiento que se va a enviar
        //movimiento = new Movimiento(Movimiento.TipoMovimiento.Vender, cartasMovidas.ToArray());

        LimpiarSeleccion();
    }

    public void Trueque() {

        //Revisar que sea el mismo numero de cartas en los dos lados
        int cartasSelecionadasMercado = 0;
        int cartasSeleccionadasJugador = 0;

        foreach (Carta carta in cartasSeleccionadas) {
            if (carta.grupo == mazoMercado) {
                cartasSelecionadasMercado++;
            } else if (carta.grupo == mazoJugador || carta.grupo == mazoJugadorCamellos) {
                cartasSeleccionadasJugador++;
            }
        }

        if (cartasSelecionadasMercado > cartasSeleccionadasJugador) {
            Debug.Log("¡No puedes recibir mas cartas que las que ofreces!");
            return;
        } else if (cartasSelecionadasMercado < cartasSeleccionadasJugador) {
            Debug.Log("¡No puedes recibir menos cartas que las que ofreces!");
            return;
        }

        //Revisar que sean dos cambios minimo
        if (cartasSeleccionadas.Count < 4) {
            Debug.Log("¡Solo puedes hacer 2 cambios o más!");
            return;
        }

        //Revisar que no se cambie por cartas del mismo tipo
        List<Carta.TipoMercancia> materialesEnJugador = new List<Carta.TipoMercancia>();
        List<Carta.TipoMercancia> materialesEnMercado = new List<Carta.TipoMercancia>();

        foreach (Carta carta in cartasSeleccionadas) {
            if (carta.grupo == mazoJugador || carta.grupo == mazoJugadorCamellos) {
                materialesEnJugador.Add(carta.mercancia);
            } else if (carta.grupo == mazoMercado) {
                materialesEnMercado.Add(carta.mercancia);
            }
        }

        for (int x = 0; x < materialesEnJugador.Count; x++) {
            for (int y = 0; y < materialesEnMercado.Count; y++) {
                if (materialesEnJugador[x] == materialesEnMercado[y]) {
                    Debug.Log("¡No puedes cambiar por cartas del mismo tipo de material!");
                    return;
                }
            }
        }

        //Hacer el trueque
        foreach (Carta carta in cartasSeleccionadas) {

            //Agregar la carta a la lista de cartas movidas para posteriormente mandarlas al otro cliente.
            cartasMovidas.Add(carta.id);

            if (carta.grupo == mazoMercado) {
                DarAJugador(carta.gameObject);
            } else if (carta.grupo == mazoJugador || carta.grupo == mazoJugadorCamellos) {
                carta.transform.SetParent(mazoMercado.transform);
            }
        }

        //Generar el objeto de tipo Movimiento que se va a enviar
        //movimiento = new Movimiento(Movimiento.TipoMovimiento.Tomar, cartasMovidas.ToArray());

        LimpiarSeleccion();
    }

    #endregion

    #region Chequeo

    bool PuedeAceptar(int cartasNormales, int camellos) {
        if (mazoJugador.transform.childCount + cartasNormales <= 7 && mazoJugadorCamellos.transform.childCount + camellos <= 7) {
            return true;
        } else {
            return false;
        }
    }

    #endregion

    #region Interfaz

    void EstadoPanelAcciones(bool estado) {
        panelAcciones.SetActive(estado);
    }

    #endregion

    #region Corrutinas

    IEnumerator Juego() {
        yield return new WaitForSeconds(1f);

        //Si es PRUEBA
        if (servidor == null && cliente == null) {
            mazoPrincipal.RevolverCartas();
        }

        //Si es ANFITRIÓN
        else if(servidor != null) {
            //Revolver las cartas
            mazoPrincipal.RevolverCartas();

            //Mandar orden de las cartas al otro cliente
            //Movimiento movOrden = new Movimiento(Movimiento.TipoMovimiento.OrdenMazoPrincipal, GetOrdenGrupo(mazoPrincipal));
            //cliente.EnviarMovimiento(movOrden);
        } 
        
        //Si es CLIENTE
        else {
            /*while (cliente.SolicitarOrdenGrupoPrincipal() == null) {
                yield return new WaitForEndOfFrame();
            }*/
            //mazoPrincipal.OrdenarCartasPorId(cliente.SolicitarOrdenGrupoPrincipal());

        }

        //Agregar los 3 camellos iniciales y llenar el mercado de cartas
        AgregarCamellosIniciales();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LlenarMercadoAnimado());
        StartCoroutine(OrdenarFichasAnimado());
        yield return new WaitForSeconds(1f);

        //Repartir sus 5 cartas a los jugadores
        StartCoroutine(RepartirAJugadoresAnimado());
        yield return new WaitWhile(() => ocupado);

        Debug.Log("Listo!");
        EstadoPanelAcciones(true);

        yield return new WaitForEndOfFrame();
    }

    IEnumerator RepartirAJugadoresAnimado() {

        ocupado = true;

        //Repartir al jugador
        foreach (GameObject x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            DarAJugador(x);
            yield return new WaitForSeconds(TIEMPO_ENTRE_CARTA_ANIMADA);
        }

        //Repartir al oponente
        foreach (GameObject x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            DarAOponente(x);
            yield return new WaitForSeconds(TIEMPO_ENTRE_CARTA_ANIMADA);
        }

        ocupado = false;

        yield return new WaitForEndOfFrame();
    }

    IEnumerator LlenarMercadoAnimado() {
        int necesarias = 5 - mazoMercado.hijos.Length;
        for (int x = 0; x < necesarias; x++) {
            mazoPrincipal.ObtenerUltimaCarta().transform.SetParent(mazoMercado.gameObject.transform);
            yield return new WaitForSeconds(TIEMPO_ENTRE_CARTA_ANIMADA);
        }
    }

    IEnumerator OrdenarFichasAnimado() {
        ocupado = true;
        List<Transform> temp = new List<Transform>();

        foreach (Transform hijo in fichasPrincipal.transform) {
            temp.Add(hijo);
        }

        foreach (Transform hijo in temp) {

            Ficha ficha = hijo.GetComponent<Ficha>();

            switch (ficha.tipoFicha) {
                case Ficha.TipoFicha.Diamante:
                    ficha.transform.SetParent(fichasDiamante.transform);
                    break;
                case Ficha.TipoFicha.Oro:
                    ficha.transform.SetParent(fichasOro.transform);
                    break;
                case Ficha.TipoFicha.Plata:
                    ficha.transform.SetParent(fichasPlata.transform);
                    break;
                case Ficha.TipoFicha.Tela:
                    ficha.transform.SetParent(fichasTela.transform);
                    break;
                case Ficha.TipoFicha.Especias:
                    ficha.transform.SetParent(fichasEspecias.transform);
                    break;
                case Ficha.TipoFicha.Cuero:
                    ficha.transform.SetParent(fichasCuero.transform);
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        ocupado = false;
    }

    #endregion

    #region Multijugador

    Cliente cliente;
    Servidor servidor;
    Movimiento movimiento;
    List<int> cartasMovidas = new List<int>();

    public void EjecutarMovimientoOponente(Movimiento movimiento) {
        turnoJugador = false;
    }

    public void MandarMovimientoJugador(Movimiento movimiento) {
        //cliente.EnviarMovimiento(movimiento);
        cartasMovidas.Clear();
    }

    public int[] GetOrdenGrupo(Grupo grupo) {
        int[] orden = new int[grupo.hijosCarta.Length];

        for (int x = 0; x < orden.Length; x++) {
            orden[x] = grupo.hijosCarta[x].id;
        }

        return orden;
    }

    #endregion

    #region Utiles

    public Carta[] ObtenerTodasLasCartas() {
        return FindObjectsOfType<Carta>();
    }

    #endregion
}
