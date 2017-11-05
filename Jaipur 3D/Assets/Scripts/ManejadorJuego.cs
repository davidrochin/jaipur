using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ManejadorJuego : MonoBehaviour {

    [Header("Partida")]
    public bool turnoJugador = true;
    public int ronda = 1;

    [Header("Grupos")]
    public Grupo mazoMercado;
    public Grupo mazoPrincipal, mazoDescartar, mazoJugador, mazoJugadorCamellos, mazoOponente, mazoOponenteCamellos;

    [Header("Fichas")]
    public Grupo fichasPrincipal;
    public Grupo fichasDiamante, fichasOro, fichasPlata, fichasTela, fichasEspecias, fichasCuero, fichasJugador, fichasOponente;

    [Header("Interfaz")]
    public GameObject panelJuego;

    [Header("Selección")]
    Seleccion seleccion;

    Jugador jugador;

    //Esta variable es la que prevee que el juego siga avanzando si se está ejecutando alguna animación, o algo por el estilo.
    private bool ocupado = false;

    private const float TIEMPO_ENTRE_CARTA_ANIMADA = 0.3f;

    void Awake() {

        //Buscar el componente Seleccion
        seleccion = FindObjectOfType<Seleccion>();

        //Inicializar elementos de la interfaz
        if (panelJuego == null) { panelJuego = GameObject.Find("panel_acciones"); }
        EstadoPanel(false);

        //Obtener el cliete y servidor
        cliente = FindObjectOfType<Cliente>();
        servidor = FindObjectOfType<Servidor>();
        networkManager = FindObjectOfType<NetworkManager>();

        //Entrar al cliente y darle la referencia de este ManejadorJuego
        if(cliente != null) { cliente.manejadorJuego = this; }

        //Determinar que tipo de jugador es este
        DeterminarTipoJugador();
        DeterminarTurno();

        //Obtener el Jugador
        jugador = FindObjectOfType<Jugador>();
        
    }

    void Start() {
        //Iniciar el juego
        StartCoroutine(Juego());
    }

    #region Inicial

    public void AgregarCamellosIniciales() {
        Debug.Log("[ManejadorJuego]: Agregando camellos iniciales.");
        foreach (GameObject camello in mazoPrincipal.ObtenerCamellos(3)) {
            camello.transform.SetParent(mazoMercado.transform);
        }
    }

    public void LlenarMercado() {
        Debug.Log("[ManejadorJuego]: Llenando mercado.");
        int necesarias = 5 - mazoMercado.hijos.Length;
        for (int x = 0; x < necesarias; x++) {
            GameObject carta = mazoPrincipal.ObtenerUltimaCarta();
            if (carta != null) { carta.transform.SetParent(mazoMercado.gameObject.transform); }

        }
    }

    public void RepartirAJugadores() {
        Debug.Log("[ManejadorJuego]: Repartiendo a jugadores.");

        //Repartir al jugador
        foreach (GameObject x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            DarCartaAJugador(x);
        }

        //Repartir al oponente
        foreach (GameObject x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            DarCartaAOponente(x);
        }

    }

    public void OrdenarFichas() {

        //Ficha[] todasFichas = ObtenerTodasLasFichas();
        Ficha[] todasFichas = fichasPrincipal.ObtenerFichas();

        foreach (Ficha ficha in todasFichas) {
            Grupo grupoDestino = fichasCuero;

            switch (ficha.tipoFicha) {
                case Ficha.TipoFicha.Diamante:
                    grupoDestino = fichasDiamante;
                    break;
                case Ficha.TipoFicha.Oro:
                    grupoDestino = fichasOro;
                    break;
                case Ficha.TipoFicha.Plata:
                    grupoDestino = fichasPlata;
                    break;
                case Ficha.TipoFicha.Tela:
                    grupoDestino = fichasTela;
                    break;
                case Ficha.TipoFicha.Especias:
                    grupoDestino = fichasEspecias;
                    break;
                case Ficha.TipoFicha.Cuero:
                    grupoDestino = fichasCuero;
                    break;
            }

            ficha.EnviarAGrupo(grupoDestino);
        }
    }

    #endregion

    #region Acciones del Jugador

    public void TomarUna() {

        //Revisar si es el turno del jugador
        if (!turnoJugador) {
            ImprimirPanelMensaje("¡Aún no es tu turno!");
            return;
        }

        //Lista para mandar las cartas movidas
        List<int> cartasMovidas = new List<int>();

        //Revisar si son solo camellos
        bool sonSoloCamellos = true;
        foreach (Carta carta in seleccion.cartasSeleccionadas) {
            if (carta.mercancia != Carta.TipoMercancia.Camello) {
                sonSoloCamellos = false;
            }
        }

        //Revisar que solo se haya seleccionado 1 carta si no son solo camellos
        if (sonSoloCamellos == false) {
            if (seleccion.cartasSeleccionadas.Count < 1) {
                ImprimirPanelMensaje("Necesitas seleccionar al menos una carta.");
                return;
            } else if (seleccion.cartasSeleccionadas.Count > 1) {
                ImprimirPanelMensaje("No puedes tomar mas de una carta, al menos sean camellos.");
                return;
            }
        }

        Debug.Log(mazoMercado);
        //Revisar si la carta es del mercado
        if (seleccion.cartasSeleccionadas[0].grupo != mazoMercado) {
            ImprimirPanelMensaje("Solo puedes tomar cartas del mercado.");
            return;
        }

        //Revisar que el jugador pueda aceptar las cartas
        int cantidadCartas = 0; int cantidadCamellos = 0;
        foreach (Carta carta in seleccion.cartasSeleccionadas) {
            if (carta.mercancia != Carta.TipoMercancia.Camello) {
                cantidadCartas++;
            } else {
                cantidadCamellos++;
            }
        }
        if (PuedeAceptar(cantidadCartas, cantidadCamellos) == false) {
            ImprimirPanelMensaje("Solo puedes tener 7 cartas normales y 7 cartas de camello.");
            return;
        }

        //Todo está bien
        foreach (GameObject objeto in seleccion.objetosSeleccionados) {
            cartasMovidas.Add(objeto.GetComponent<Carta>().id);
            DarCartaAJugador(objeto);
        }

        //Generar el objeto tipo Movimiento que se va a enviar
        Movimiento mov = new Movimiento();
        mov.ids = cartasMovidas.ToArray();
        mov.tipoMovimiento = Movimiento.TipoMovimiento.Tomar;
        cliente.EnviarMovimiento(mov);
        //movimiento = new Movimiento(Movimiento.TipoMovimiento.Tomar, cartasMovidas.ToArray());

        seleccion.LimpiarSeleccion();
        LlenarMercado();
        TerminarTurno();
    }

    public void Vender() {

        //Revisar si es el turno del jugador
        if (!turnoJugador) {
            ImprimirPanelMensaje("¡Aún no es tu turno!");
            return;
        }

        //Lista para mandar las cartas movidas
        List<int> cartasMovidas = new List<int>();

        //Revisar que haya cartas seleccionadas
        if (seleccion.cartasSeleccionadas.Count < 1) {
            ImprimirPanelMensaje("¡Necesitas seleccionar al menos una carta!");
            return;
        }

        Carta.TipoMercancia primerMaterial = seleccion.cartasSeleccionadas[0].mercancia;
        foreach (Carta carta in seleccion.cartasSeleccionadas) {

            //Revisar que las cartas no sean del mercado
            if (carta.grupo == mazoMercado) {
                ImprimirPanelMensaje("¡No puedes vender cartas del mercado!");
                return;
            }

            //Revisar que no sean camellos
            if (carta.mercancia == Carta.TipoMercancia.Camello) {
                ImprimirPanelMensaje("¡No puedes vender camellos!");
                return;
            }

            //Revisar que sean del mismo material
            if (carta.mercancia != primerMaterial) {
                ImprimirPanelMensaje("¡No puedes vender cartas de diferente tipo de material!");
                return;
            }
        }

        //Todo en orden. Vender las cartas
        foreach (Carta carta in seleccion.cartasSeleccionadas) {
            cartasMovidas.Add(carta.id);
            DarFichaPorCarta(carta, fichasJugador);
            carta.transform.SetParent(mazoDescartar.transform);
        }

        //Generar el objeto de tipo Movimiento que se va a enviar
        Movimiento mov = new Movimiento();
        mov.ids = cartasMovidas.ToArray();
        mov.tipoMovimiento = Movimiento.TipoMovimiento.Vender;
        cliente.EnviarMovimiento(mov);

        seleccion.LimpiarSeleccion();
        TerminarTurno();
    }

    public void Trueque() {

        //Revisar si es el turno del jugador
        if (!turnoJugador) {
            ImprimirPanelMensaje("¡Aún no es tu turno!");
            return;
        }

        //Lista para mandar las cartas movidas
        List<int> cartasMovidas = new List<int>();

        //Revisar que sea el mismo numero de cartas en los dos lados
        int cartasSelecionadasMercado = 0;
        int cartasSeleccionadasJugador = 0;

        foreach (Carta carta in seleccion.cartasSeleccionadas) {
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
        if (seleccion.cartasSeleccionadas.Count < 4) {
            Debug.Log("¡Solo puedes hacer 2 cambios o más!");
            return;
        }

        //Revisar que no se cambie por cartas del mismo tipo
        List<Carta.TipoMercancia> materialesEnJugador = new List<Carta.TipoMercancia>();
        List<Carta.TipoMercancia> materialesEnMercado = new List<Carta.TipoMercancia>();

        foreach (Carta carta in seleccion.cartasSeleccionadas) {
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
        foreach (Carta carta in seleccion.cartasSeleccionadas) {

            //Agregar la carta a la lista de cartas movidas para posteriormente mandarlas al otro cliente.
            cartasMovidas.Add(carta.id);

            if (carta.grupo == mazoMercado) {
                DarCartaAJugador(carta.gameObject);
            } else if (carta.grupo == mazoJugador || carta.grupo == mazoJugadorCamellos) {
                carta.transform.SetParent(mazoMercado.transform);
            }
        }

        //Generar el objeto de tipo Movimiento que se va a enviar
        Movimiento mov = new Movimiento();
        mov.ids = cartasMovidas.ToArray();
        mov.tipoMovimiento = Movimiento.TipoMovimiento.Trueque;
        cliente.EnviarMovimiento(mov);

        seleccion.LimpiarSeleccion();
        TerminarTurno();
    }

    #endregion

    #region Acciones del Manejador

    public void DarCartaAJugador(GameObject carta) {
        if (carta.GetComponent<Carta>().mercancia != Carta.TipoMercancia.Camello) {
            carta.transform.SetParent(mazoJugador.transform);
            mazoJugador.OrdenarPorTipo();
        } else {
            carta.transform.SetParent(mazoJugadorCamellos.transform);
        }
    }

    public void DarCartaAOponente(GameObject carta) {
        if (carta.GetComponent<Carta>().mercancia != Carta.TipoMercancia.Camello) {
            carta.transform.SetParent(mazoOponente.transform);
        } else {
            carta.transform.SetParent(mazoOponenteCamellos.transform);
        }
    }

    public void DarFichaPorCarta(Carta cartaVendida, Grupo grupoDestino) {

        Grupo grupoFichasCorrecto = fichasCuero;

        switch (cartaVendida.mercancia) {
            case Carta.TipoMercancia.Cuero:
                grupoFichasCorrecto = fichasCuero;
                break;
            case Carta.TipoMercancia.Especias:
                grupoFichasCorrecto = fichasEspecias;
                break;
            case Carta.TipoMercancia.Tela:
                grupoFichasCorrecto = fichasTela;
                break;
            case Carta.TipoMercancia.Plata:
                grupoFichasCorrecto = fichasPlata;
                break;
            case Carta.TipoMercancia.Oro:
                grupoFichasCorrecto = fichasOro;
                break;
            case Carta.TipoMercancia.Diamante:
                grupoFichasCorrecto = fichasDiamante;
                break;
        }

        if (grupoFichasCorrecto.ObtenerCantidadDeHijos() > 0) {
            Ficha ficha = grupoFichasCorrecto.ObtenerUltimaFicha();
            ficha.EnviarAGrupo(grupoDestino);
            jugador.AgregarFichas(ficha.valorFicha);
        } else {
            Debug.LogWarning("El jugador vendió una Carta pero no hay fichas para entregarle.");
        }
    }

    public void DarFichasPorCartas(Carta[] cartasVendidas, Grupo grupoDestino) {
        foreach (Carta carta in cartasVendidas) {
            DarFichaPorCarta(carta, grupoDestino);
        }
    }

    #endregion

    #region Interfaz

    void EstadoPanel(bool estado) {
        panelJuego.SetActive(estado);
    }

    #endregion

    #region Corrutinas

    IEnumerator Juego() {
        yield return new WaitForSeconds(1f);
       
        if(servidor == null && cliente == null) {
            mazoPrincipal.RevolverCartas();
        }

        //Si es ANFITRIÓN
        if (tipoJugador == TipoJugador.Anfitrion) {
            //Revolver las cartas
            mazoPrincipal.RevolverCartas();

            yield return new WaitForEndOfFrame();

            //Mandar orden de las cartas al otro cliente
            Movimiento mov = new Movimiento();
            mov.tipoMovimiento = Movimiento.TipoMovimiento.OrdenMazoPrincipal;
            mov.ids = mazoPrincipal.ObtenerOrdenPorId();
            cliente.EnviarMovimiento(mov);
        }

        //Si es solo CLIENTE
        if (tipoJugador == TipoJugador.Invitado) {
            yield return new WaitUntil(() => cartasBarajeadas);
        }

        //Agregar los 3 camellos iniciales y llenar el mercado de cartas
        AgregarCamellosIniciales();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LlenarMercadoAnimado());
        //StartCoroutine(OrdenarFichasAnimado());
        OrdenarFichas();
        yield return new WaitForSeconds(1f);

        //Repartir sus 5 cartas a los jugadores
        StartCoroutine(RepartirAJugadoresAnimado());
        yield return new WaitWhile(() => ocupado);

        Debug.Log("Listo!");
        EstadoPanel(true);

        yield return new WaitForEndOfFrame();
    }

    IEnumerator RepartirAJugadoresAnimado() {
        //Debug.Log("[ManejadorJuego]: Repartiendo a jugadores.");
        ocupado = true;

        //Repartir al jugador
        foreach (GameObject x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            if(tipoJugador == TipoJugador.Anfitrion) {
                DarCartaAJugador(x);
            } else {
                DarCartaAOponente(x);
            }
            
            yield return new WaitForSeconds(TIEMPO_ENTRE_CARTA_ANIMADA);
        }

        //Repartir al oponente
        foreach (GameObject x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            if (tipoJugador == TipoJugador.Anfitrion) {
                DarCartaAOponente(x);
            } else {
                DarCartaAJugador(x);
            }
            
            yield return new WaitForSeconds(TIEMPO_ENTRE_CARTA_ANIMADA);
        }

        ocupado = false;

        yield return new WaitForEndOfFrame();
    }

    IEnumerator LlenarMercadoAnimado() {
        //Debug.Log("[ManejadorJuego]: Llenando mercado.");
        int necesarias = 5 - mazoMercado.hijos.Length;
        for (int x = 0; x < necesarias; x++) {
            //Debug.Log("Mandando carta #" + mazoPrincipal.ObtenerUltimaCarta().GetComponent<Carta>().id + " al mercado.");
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

    public TipoJugador tipoJugador;

    Cliente cliente;
    Servidor servidor;
    NetworkManager networkManager;

    bool cartasBarajeadas = false;

    public void DeterminarTipoJugador() {
        if(servidor != null) {
            Debug.Log("Soy Host");
            tipoJugador = TipoJugador.Anfitrion;
        } else {
            Debug.Log("Soy Invitado");
            tipoJugador = TipoJugador.Invitado;
        }
    }

    public void DeterminarTurno() {
        if (tipoJugador == TipoJugador.Anfitrion) {
            if (ronda == 1) {
                turnoJugador = true;
            } else {
                turnoJugador = false;
            }
        } else if (tipoJugador == TipoJugador.Invitado) {
            if (ronda == 1) {
                turnoJugador = false;
            } else {
                turnoJugador = true;
            }
        }
        ActualizarMensajeTurno();
    }

    public void EjecutarMovimientoOponente(Movimiento movimiento) {

        //Imprimir el movimiento en consola
        Debug.Log(movimiento);

        //Obtener todas las cartas del juego
        Carta[] todasCartas = ObtenerTodasLasCartas();

        switch (movimiento.tipoMovimiento) {
            case Movimiento.TipoMovimiento.OrdenMazoPrincipal:
                mazoPrincipal.OrdenarCartasPorId(movimiento.ids);
                cartasBarajeadas = true;
                break;
            case Movimiento.TipoMovimiento.Tomar:
                foreach (Carta carta in todasCartas) {
                    //Ver si la carta se movió
                    if (movimiento.SeEncuentraId(carta.id)) {
                        //Mandarla a su mazo correspondiente
                        if (carta.mercancia == Carta.TipoMercancia.Camello) {
                            carta.transform.SetParent(mazoOponenteCamellos.transform);
                        } else {
                            carta.transform.SetParent(mazoOponente.transform);
                        }
                    }
                }
                LlenarMercado();
                break;
            case Movimiento.TipoMovimiento.Vender:
                foreach (Carta carta in todasCartas) {
                    //Revisar si la carta se movió
                    if (movimiento.SeEncuentraId(carta.id)) {
                        //Mandarla a su mazo correspondiente
                        carta.transform.SetParent(mazoDescartar.transform);
                        DarFichaPorCarta(carta, fichasOponente);
                    }
                }
                break;
            case Movimiento.TipoMovimiento.Trueque:
                foreach (Carta carta in todasCartas) {
                    //Revisar si la carta se movió
                    if (movimiento.SeEncuentraId(carta.id)) {
                        //Mandarla a su mazo correspondiente
                        if(carta.grupo == mazoOponente || carta.grupo == mazoOponenteCamellos) {
                            carta.transform.SetParent(mazoMercado.transform);
                        }
                        else if(carta.grupo == mazoMercado) {
                            DarCartaAOponente(carta.gameObject);
                        }
                    }
                }
                break;
        }

        if(movimiento.tipoMovimiento != Movimiento.TipoMovimiento.OrdenMazoPrincipal) {
            EmpezarTurno();
        }
    }

    public int[] GetOrdenGrupo(Grupo grupo) {
        int[] orden = new int[grupo.hijosCarta.Length];

        for (int x = 0; x < orden.Length; x++) {
            orden[x] = grupo.hijosCarta[x].id;
        }

        return orden;
    }

    public void TerminarTurno() {
        turnoJugador = false;
        ActualizarMensajeTurno();
    }

    public void EmpezarTurno() {
        turnoJugador = true;
        ActualizarMensajeTurno();
    }

    public enum TipoJugador { Anfitrion, Invitado }

    #endregion

    #region Utiles

    public bool PuedeAceptar(int cartasNormales, int camellos) {
        if (mazoJugador.transform.childCount + cartasNormales <= 7 && mazoJugadorCamellos.transform.childCount + camellos <= 7) {
            return true;
        } else {
            return false;
        }
    }

    public Carta[] ObtenerTodasLasCartas() {
        return FindObjectsOfType<Carta>();
    }

    public Ficha[] ObtenerTodasLasFichas() {
        return FindObjectsOfType<Ficha>();
    }

    public Ficha[] ObtenerTodasFichasDeTipo(Ficha.TipoFicha tipoFichas) {
        Ficha[] todasFichas = ObtenerTodasLasFichas();
        List<Ficha> fichasDelTipo = new List<Ficha>();
        foreach (Ficha ficha in todasFichas) {
            if(ficha.tipoFicha == tipoFichas && ficha.grupo != fichasJugador && ficha.grupo != fichasOponente) {
                fichasDelTipo.Add(ficha);
            }
        }
        return fichasDelTipo.ToArray();
    }

    public Carta ObtenerCartaPorId(int id) {
        Carta[] todasCartas = ObtenerTodasLasCartas();
        foreach (Carta carta in todasCartas) {
            if(carta.id == id) {
                return carta;
            }
        }
        return null;
    }

    public void ImprimirPanelMensaje(string texto) {
        GameObject.FindGameObjectWithTag("Mensaje Panel").GetComponent<UnityEngine.UI.Text>().text = texto;
    }

    public void ActualizarMensajeTurno() {
        UnityEngine.UI.Text texto = GameObject.FindGameObjectWithTag("Mensaje Turno").GetComponent<UnityEngine.UI.Text>();

        if (turnoJugador) {
            texto.text = "Es tu turno";
        } else {
            texto.text = "Es el turno de tu oponente";
        }
    }

    #endregion

    private void OnGUI() {
        if(servidor == null && cliente == null) {
            if (GUILayout.Button("Iniciar Prueba")) {
                FindObjectOfType<ManejadorRed>().ConfigurarComoHost();
                FindObjectOfType<ManejadorRed>().ConfigurarComoCliente();
            }
        }
    }
}
