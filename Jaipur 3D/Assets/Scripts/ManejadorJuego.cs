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
    public Grupo fichasDiamante, fichasOro, fichasPlata, fichasTela, fichasEspecias, fichasCuero, fichasJugador, fichasOponente, fichasCamello, fichasBonus3, fichasBonus4, fichasBonus5;

    [Header("Interfaz")]
    public GameObject panelJuego;

    [Header("Selección")]
    Seleccion seleccion;

    [Header("Jugadores")]
    public Jugador jugador;
    public Jugador oponente;
    public Jugador ultimoGanador;

    //Esta variable es la que prevee que el juego siga avanzando si se está ejecutando alguna animación, o algo por el estilo.
    private bool ocupado = false;

    private const float TIEMPO_ENTRE_CARTA_ANIMADA = 0.3f;

    void Awake() {
        //Buscar el componente Seleccion
        seleccion = FindObjectOfType<Seleccion>();

        //Inicializar elementos de la interfaz
        if (panelJuego == null) { panelJuego = GameObject.Find("panel_acciones"); }
        MostrarBotonesAccion(false);

        //Obtener el cliete y servidor
        cliente = FindObjectOfType<Cliente>();
        servidor = FindObjectOfType<Servidor>();

        //Entrar al cliente y darle la referencia de este ManejadorJuego
        if(cliente != null) { cliente.manejadorJuego = this; }

        //Inicializar los objetos Jugador
        jugador = new Jugador();
        oponente = new Jugador();

        //Determinar que tipo de jugador es este
        DeterminarTipoJugador();
        DeterminarTurno();
        
    }

    void Start() {

        ActualizarContadorFichas();

        //Iniciar el juego
        StartCoroutine(Juego());
    }

    #region Inicial

    public void AgregarCamellosIniciales() {
        Debug.Log("[ManejadorJuego]: Agregando camellos iniciales.");
        foreach (Carta camello in mazoPrincipal.ObtenerCamellos(3)) {
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
        foreach (Carta x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            DarCartaAJugador(x);
        }

        //Repartir al oponente
        foreach (Carta x in mazoPrincipal.ObtenerUltimasCartas(5)) {
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
                case Ficha.TipoFicha.Camello:
                    grupoDestino = fichasCamello;
                    break;
                case Ficha.TipoFicha.Bonus3:
                    grupoDestino = fichasBonus3;
                    break;
                case Ficha.TipoFicha.Bonus4:
                    grupoDestino = fichasBonus4;
                    break;
                case Ficha.TipoFicha.Bonus5:
                    grupoDestino = fichasBonus5;
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

        //Revisar si la carta es del mercado
        if (seleccion.cartasSeleccionadas[0].grupo != mazoMercado) {
            ImprimirPanelMensaje("Solo puedes tomar cartas del mercado.");
            return;
        }

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
                ImprimirPanelMensaje("No puedes tomar más de una carta, al menos que sean camellos.");
                return;
            }
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
            ImprimirPanelMensaje("Solo puedes tener 7 cartas de mercancía.");
            return;
        }

        //Todo está bien
        foreach (GameObject objeto in seleccion.objetosSeleccionados) {
            Carta carta = objeto.GetComponent<Carta>();
            cartasMovidas.Add(carta.id);
            DarCartaAJugador(carta);
        }

        if(seleccion.cartasSeleccionadas[0].mercancia == Carta.TipoMercancia.Camello) {
            ImprimirPanelMensaje("Has tomado todos los camellos.");
        } else {
            ImprimirPanelMensaje("Has tomado una carta de " + seleccion.cartasSeleccionadas[0].mercancia.ToString() + ".");
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

        //Guardar el material de la primera carta para checar que todas sean iguales
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

        //Si es Plata, Oro o Diamante, revisar que sean minimo 2
        if(primerMaterial == Carta.TipoMercancia.Plata || primerMaterial == Carta.TipoMercancia.Oro || primerMaterial == Carta.TipoMercancia.Diamante) {
            if(seleccion.cartasSeleccionadas.Count < 2) {
                ImprimirPanelMensaje("¡Necesitas mínimo 2 cartas de este tipo de mercancía para vender!");
                return;
            }
        }

        //Todo en orden. Vender las cartas
        DarFichasPorCartas(seleccion.cartasSeleccionadas.ToArray(), fichasJugador);

        foreach (Carta carta in seleccion.cartasSeleccionadas) {
            carta.EnviarAGrupo(mazoDescartar);
        }

        ImprimirPanelMensaje("Has vendido " + seleccion.cartasSeleccionadas.Count + " carta(s) de " + seleccion.cartasSeleccionadas[0].mercancia.ToString() + ".");

        //Generar el objeto de tipo Movimiento que se va a enviar
        Movimiento mov = new Movimiento();
        mov.ids = seleccion.ObtenerSeleccionadasPorId();
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
            ImprimirPanelMensaje("¡No puedes recibir más cartas que las que ofreces!");
            return;
        } else if (cartasSelecionadasMercado < cartasSeleccionadasJugador) {
            ImprimirPanelMensaje("¡No puedes recibir menos cartas que las que ofreces!");
            return;
        }

        //Revisar que sean dos cambios minimo
        if (seleccion.cartasSeleccionadas.Count < 4) {
            ImprimirPanelMensaje("¡Solo puedes hacer 2 cambios o más!");
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
                    ImprimirPanelMensaje("¡No puedes cambiar por cartas del mismo tipo de material!");
                    return;
                }
            }
        }

        //Revisar que no esten seleccionados camellos del mercado
        foreach (Carta carta in seleccion.cartasSeleccionadas) {
            if(carta.mercancia == Carta.TipoMercancia.Camello && carta.grupo == mazoMercado) {
                ImprimirPanelMensaje("¡No puedes cambiar cartas por camellos del mercado!");
                return;
            }
        }

        //Revisar que despues del cambio, el jugador no superará el limite de 7 cartas.
        int cantSeleccionadasMercado = 0;
        int cantSeleccionadasJugador = 0;
        foreach (Carta carta in seleccion.cartasSeleccionadas) {
            if(carta.grupo == mazoMercado) { cantSeleccionadasMercado++; }
            if (carta.grupo == mazoJugador) { cantSeleccionadasJugador++; }
        }

        if(!PuedeAceptar(cantSeleccionadasMercado - cantSeleccionadasJugador, 0)) {
            ImprimirPanelMensaje("¡No puedes tener más de 7 cartas de mercancía!");
            return;
        }

        //Todo está bien. Hacer el trueque
        foreach (Carta carta in seleccion.cartasSeleccionadas) {

            //Agregar la carta a la lista de cartas movidas para posteriormente mandarlas al otro cliente.
            cartasMovidas.Add(carta.id);

            if (carta.grupo == mazoMercado) {
                DarCartaAJugador(carta);
            } else if (carta.grupo == mazoJugador || carta.grupo == mazoJugadorCamellos) {
                carta.transform.SetParent(mazoMercado.transform);
            }
        }

        ImprimirPanelMensaje("Has cambiado " + seleccion.cartasSeleccionadas.Count / 2 + " cartas por " + seleccion.cartasSeleccionadas.Count / 2 + " cartas.");

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

    public void DarCartaAJugador(Carta carta) {
        if (carta.GetComponent<Carta>().mercancia != Carta.TipoMercancia.Camello) {
            carta.transform.SetParent(mazoJugador.transform);
            mazoJugador.OrdenarCartasPorTipo();
        } else {
            carta.transform.SetParent(mazoJugadorCamellos.transform);
        }
    }

    public void DarCartaAOponente(Carta carta) {
        if (carta.GetComponent<Carta>().mercancia != Carta.TipoMercancia.Camello) {
            carta.transform.SetParent(mazoOponente.transform);
            mazoOponente.OrdenarCartasPorTipo();
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

        //Revisar si hay fichas disponibles
        if (grupoFichasCorrecto.ObtenerCantidadDeHijos() > 0) {

            //Obtener la ultima ficha y mandarla al grupo especificado
            Ficha ficha = grupoFichasCorrecto.ObtenerUltimaFicha();
            ficha.EnviarAGrupo(grupoDestino);

            //Sumar el contador de fichas del jugador (u oponente)
            if(grupoDestino == fichasJugador) {
                jugador.SumarFicha(ficha.valorFicha);
                jugador.fichasProducto++;
            } else if (grupoDestino == fichasOponente) {
                oponente.SumarFicha(ficha.valorFicha);
                oponente.fichasProducto++;
            }
            
        } else {
            Debug.LogWarning("El jugador vendió una Carta pero no hay fichas para entregarle.");
        }
    }

    public void DarFichasPorCartas(Carta[] cartasVendidas, Grupo grupoDestino) {

        //Vender cada una de las cartas
        foreach (Carta carta in cartasVendidas) {
            //Debug.Log("Vendiendo " + carta.mercancia.ToString());
            DarFichaPorCarta(carta, grupoDestino);
        }

        //Entregar fichas de bonificación de ser necesario
        DarFichaBonificacion(cartasVendidas.Length, grupoDestino);
    }

    public void DarFichaBonificacion(int vendidas, Grupo grupoDestino) {

        Grupo grupoCorrecto = null;

        //Determinar de que tipo de ficha Bonus se le va a dar
        switch (vendidas) {
            case 3:
                grupoCorrecto = fichasBonus3;
                break;
            case 4:
                grupoCorrecto = fichasBonus4;
                break;
            case 5:
                grupoCorrecto = fichasBonus5;
                break;
        }

        //Si vendió más de 5, darle Bonus5
        if(vendidas > 5) {
            grupoCorrecto = fichasBonus5;
        }

        if (grupoCorrecto != null && grupoCorrecto.ObtenerCantidadDeHijos() > 0) {
            Ficha fichaADar = grupoCorrecto.ObtenerUltimaFicha();

            //Sumar el contador de fichas del jugador (u oponente)
            if (grupoDestino == fichasJugador) {
                jugador.SumarFicha(fichaADar.valorFicha);
                jugador.fichasBonificacion++;
            } else if (grupoDestino == fichasOponente) {
                oponente.SumarFicha(fichaADar.valorFicha);
                oponente.fichasBonificacion++;
            }

            fichaADar.EnviarAGrupo(grupoDestino);
            fichaADar.ActualizarValor();
        }
    }

    #endregion

    #region Interfaz

    void MostrarBotonesAccion(bool mostrar) {
        panelJuego.SetActive(mostrar);
    }

    void ActualizarContadorFichas() {
        //GameObject.FindGameObjectWithTag("Texto Fichas").GetComponent<UnityEngine.UI.Text>().text = "" + fichasJugador.ContarFichas();
        GameObject.FindGameObjectWithTag("Texto Fichas").GetComponent<UnityEngine.UI.Text>().text = "" + jugador.sumaFichas;
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

    public void AbandonarPartida() {
        FindObjectOfType<ManejadorRed>().CerrarTodo();
        FindObjectOfType<ManejadorMenu>().CargarEscena("menu");
    }

    #endregion

    #region Corrutinas

    IEnumerator Juego() {
        yield return new WaitForSeconds(1f);

        OrdenarFichas();
        Debug.Log("Ya se mandaron las fichas a su lugar");

        if (servidor == null && cliente == null) {
            mazoPrincipal.RevolverCartas();
        }

        //Si es ANFITRIÓN
        if (tipoMaquina == TipoMaquina.Anfitrion) {
            //Revolver las cartas
            mazoPrincipal.RevolverCartas();

            yield return new WaitForEndOfFrame();

            //Mandar orden de las cartas al otro cliente
            Movimiento mov = new Movimiento();
            mov.tipoMovimiento = Movimiento.TipoMovimiento.OrdenMazoPrincipal;
            mov.ids = mazoPrincipal.ObtenerOrdenPorId();
            cliente.EnviarMovimiento(mov);

            cliente.EnviarOrden(fichasBonus3.RandomizarFichas());
            cliente.EnviarOrden(fichasBonus4.RandomizarFichas());
            cliente.EnviarOrden(fichasBonus5.RandomizarFichas());
        }

        //Si es solo CLIENTE
        if (tipoMaquina == TipoMaquina.Invitado) {
            yield return new WaitUntil(() => cartasBarajeadas);
        }

        //Agregar los 3 camellos iniciales y llenar el mercado de cartas
        AgregarCamellosIniciales();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LlenarMercadoAnimado());
        yield return new WaitForSeconds(1f);

        //Repartir sus 5 cartas a los jugadores
        StartCoroutine(RepartirAJugadoresAnimado());
        yield return new WaitWhile(() => ocupado);

        Debug.Log("Listo!");
        MostrarBotonesAccion(true);

        yield return new WaitForEndOfFrame();
    }

    IEnumerator RepartirAJugadoresAnimado() {
        //Debug.Log("[ManejadorJuego]: Repartiendo a jugadores.");
        ocupado = true;

        //Repartir al jugador
        foreach (Carta x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            if(tipoMaquina == TipoMaquina.Anfitrion) {
                DarCartaAJugador(x);
            } else {
                DarCartaAOponente(x);
            }
            
            yield return new WaitForSeconds(TIEMPO_ENTRE_CARTA_ANIMADA);
        }

        //Repartir al oponente
        foreach (Carta x in mazoPrincipal.ObtenerUltimasCartas(5)) {
            if (tipoMaquina == TipoMaquina.Anfitrion) {
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

    public TipoMaquina tipoMaquina;

    Cliente cliente;
    Servidor servidor;

    bool cartasBarajeadas = false;
    bool fichasOrdenadas = false;

    public void DeterminarTipoJugador() {
        if(servidor != null) {
            Debug.Log("Soy Host");
            tipoMaquina = TipoMaquina.Anfitrion;
        } else {
            Debug.Log("Soy Invitado");
            tipoMaquina = TipoMaquina.Invitado;
        }
    }

    public void DeterminarTurno() {

        //Si es el Anfitrion y es la primera ronda, determinar el turno aleatoriamente
        if (tipoMaquina == TipoMaquina.Anfitrion && ronda == 1) {
            MensajeTurno turno = new MensajeTurno();
            float ran = Random.Range(0f,1f); Debug.Log("ran = " + ran);

            //Determinar aleatoriamente el turno
            if(ran < 0.5) {
                Debug.Log("Es el turno del anfitrion");
                turno.jugador = MensajeTurno.Jugador.Anfitrion;
                turnoJugador = true;
            } else {
                Debug.Log("Es el turno del invitado");
                turno.jugador = MensajeTurno.Jugador.Invitado;
                turnoJugador = false;
            }

            //Enviarle el turno al otro jugador
            cliente.EnviarTurno(turno);
        } 
        
        //Si no es la primera ronda, determinar el turno
        //deacuerdo al perdedor de la ronda anterior
        else if(ronda != 1){
            if(ultimoGanador == jugador) {
                turnoJugador = false;
            } else if(ultimoGanador == oponente) {
                turnoJugador = true;
            }
        }

        ActualizarMensajeTurno();
    }

    public void EjecutarMovimientoOponente(Movimiento movimiento) {

        //Imprimir el movimiento en consola
        Debug.Log("[ManejadorJuego] Movimiento del oponente recibido: " + movimiento);

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

                        if(carta.mercancia == Carta.TipoMercancia.Camello) {
                            ImprimirPanelMensaje("Tu oponente ha tomado todos los camellos.");
                        } else {
                            ImprimirPanelMensaje("Tu oponente ha tomado una carta de " + carta.mercancia.ToString() + ".");
                        }
                        
                    }
                }
                LlenarMercado();
                break;
            case Movimiento.TipoMovimiento.Vender:
                List<Carta> cartasVendidas = new List<Carta>();
                foreach (Carta carta in todasCartas) {
                    //Revisar si la carta se movió
                    if (movimiento.SeEncuentraId(carta.id)) {
                        cartasVendidas.Add(carta);
                        carta.EnviarAGrupo(mazoDescartar);
                    }
                }
                ImprimirPanelMensaje("Tu oponente ha vendido " + cartasVendidas.Count + " carta(s) de " + cartasVendidas[0].mercancia.ToString() + ".");
                DarFichasPorCartas(cartasVendidas.ToArray(), fichasOponente);
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
                            DarCartaAOponente(carta);
                        }
                    }
                }
                ImprimirPanelMensaje("Tu oponente ha cambiado " + movimiento.ids.Length / 2 + " cartas por " + movimiento.ids.Length / 2 + " cartas.");
                break;
        }

        if(movimiento.tipoMovimiento != Movimiento.TipoMovimiento.OrdenMazoPrincipal) {
            EmpezarTurno();
        }

    }

    public void EjecutarOrden(MensajeOrden orden) {
        Debug.Log("Ejecutando orden en el grupo " + orden.nombreGrupo + ": " + orden);

        if (!fichasOrdenadas) {
            OrdenarFichas();
        }

        Grupo grupo = GameObject.Find(orden.nombreGrupo).GetComponent<Grupo>();
        switch (grupo.tipoGrupo) {
            case Grupo.TipoGrupo.Cartas:
                break;
            case Grupo.TipoGrupo.Fichas:
                grupo.OrdenarFichas(orden);
                break;
        }
    }

    public void EjecutarTurno(MensajeTurno turno) {
        if(turno.jugador == MensajeTurno.Jugador.Anfitrion) {
            turnoJugador = false;
        } else {
            turnoJugador = true;
        }
        ActualizarMensajeTurno();
    }

    public void TerminarTurno() {
        turnoJugador = false;
        ActualizarMensajeTurno();
        ActualizarContadorFichas();
        RevisarFinDeRonda();
    }

    public void EmpezarTurno() {
        turnoJugador = true;
        ActualizarMensajeTurno();
    }

    public void IniciarNuevaRonda() {

        //Sumar al contador de ronda
        ronda++;

        //Borrar todas las cartas y fichas del tablero. Volver a crear los set
        CreadorDeSets.BorrarTodasLasCartasYFichas();
        mazoPrincipal.GetComponent<CreadorDeSets>().CrearSet();
        fichasPrincipal.GetComponent<CreadorDeSets>().CrearSet();

        //Para la corrutina Juego
        StopCoroutine(Juego());
        cartasBarajeadas = false;
        ocupado = false;

        //Resetear los contadores de fichas
        jugador.ResetearFichas();
        oponente.ResetearFichas();
        ActualizarContadorFichas();

        //Determinar que jugador empieza y volver a empezar la corrutina Juego
        DeterminarTurno();
        StartCoroutine(Juego());
    }

    public void AcabarRonda() {
        ImprimirPanelMensaje("¡Se terminó la ronda!");

        string mensajeDialogo = "";

        //Dar la ficha de camello al jugador con mas camellos
        //CODIGO

        //Determinar ganador y entregarle un sello de excelencia
        if(jugador.sumaFichas > oponente.sumaFichas) {
            jugador.sellosDeExcelencia++;
            mensajeDialogo += "Has ganado esta ronda por mayoría de fichas.\n";
            ultimoGanador = jugador;
        } else if(jugador.sumaFichas < oponente.sumaFichas) {
            oponente.sellosDeExcelencia++;
            mensajeDialogo += "Has perdido esta ronda por minoría de fichas.\n";
            ultimoGanador = oponente;
        } 
        
        //En caso de empate de fichas
        else if (jugador.sumaFichas == oponente.sumaFichas) {
            if(jugador.fichasBonificacion > oponente.fichasBonificacion) {
                jugador.sellosDeExcelencia++;
                mensajeDialogo += "Has ganado esta ronda por mayoría de fichas de bonificación.\n";
                ultimoGanador = jugador;
            } else if (jugador.fichasBonificacion < oponente.fichasBonificacion) {
                oponente.sellosDeExcelencia++;
                mensajeDialogo += "Has perdido esta ronda por minoría de fichas de bonificación.\n";
                ultimoGanador = oponente;
            } 
            
            //En caso de empate de fichas de bonificación
            else if (jugador.fichasBonificacion == oponente.fichasBonificacion) {
                if (jugador.fichasProducto > oponente.fichasProducto) {
                    jugador.sellosDeExcelencia++;
                    mensajeDialogo += "Has ganado esta ronda por mayoría de fichas de producto.\n";
                    ultimoGanador = jugador;
                } else if (jugador.fichasProducto < oponente.fichasProducto) {
                    oponente.sellosDeExcelencia++;
                    mensajeDialogo += "Has perdido esta ronda por minoría de fichas de producto.\n";
                    ultimoGanador = oponente;
                }
            }
        }

        FindObjectOfType<CreadorDialogos>().CrearDialogo(mensajeDialogo, CreadorDialogos.AccionBoton.Nada);

        //Iniciar la siguiente ronda o acabar el juego de ser necesario
        if (jugador.sellosDeExcelencia < 2 && oponente.sellosDeExcelencia < 2) {
            IniciarNuevaRonda();
        } else {
            AcabarJuego();
        }
    }

    public void AcabarJuego() {
        string dialogoFin;

        if (jugador.sellosDeExcelencia > oponente.sellosDeExcelencia) {
            dialogoFin = "¡Felicidades! Has ganado esta partida.";
        } else if (jugador.sellosDeExcelencia < oponente.sellosDeExcelencia) {
            dialogoFin = "Has perdido esta partida. ¡Mejor suerte a la próxima!";
        } else {
            dialogoFin = "¡Empate!";
        }
        FindObjectOfType<ManejadorRed>().DesactivarCorrutinasDesconexion();
        FindObjectOfType<CreadorDialogos>().CrearDialogo(dialogoFin, CreadorDialogos.AccionBoton.SalirAlMenu);
    }

    public void RevisarFinDeRonda() {

        bool acabarRonda = false;

        //Contar cuantos grupos de fichas se han acabado
        int gruposFichasAcabados = 0;
        foreach (Grupo grupo in FindObjectsOfType<Grupo>()) {
            if(!grupo.ignorarAlContar && grupo.tipoGrupo == Grupo.TipoGrupo.Fichas && grupo.ObtenerCantidadDeHijos() <= 0) {
                gruposFichasAcabados++;
            }
        }

        //Si se acabaron 3 o se acabaron las cartas del mazo principal, terminar la ronda
        if(gruposFichasAcabados >= 3 || mazoPrincipal.ObtenerCantidadDeHijos() <= 0) {
            acabarRonda = true;
        }

        if (acabarRonda) {
            cliente.EnviarAccion(MensajeAccion.TipoAccion.AcabarRonda);
            AcabarRonda();
        }
    }

    public enum TipoMaquina { Anfitrion, Invitado, Prueba }

    #endregion

    #region Utiles

    public bool PuedeAceptar(int cartasNormales, int camellos) {
        if (mazoJugador.transform.childCount + cartasNormales <= 7 && mazoJugadorCamellos.transform.childCount + camellos <= 11) {
            return true;
        } else {
            return false;
        }
    }

    public bool PuedeAceptar(Carta[] cartas) {
        int cuentaMercancias = 0;

        foreach (Carta carta in cartas) {
            if(carta.mercancia != Carta.TipoMercancia.Camello) {
                cuentaMercancias++;
            }
        }

        if(mazoJugador.ObtenerCantidadDeHijos() + cuentaMercancias > 7) {
            return false;
        } else {
            return true;
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

    #endregion

    private void OnGUI() {
        if(servidor == null && cliente == null && Application.platform == RuntimePlatform.WindowsEditor) {
            if (GUILayout.Button("Iniciar Prueba")) {
                FindObjectOfType<ManejadorRed>().ConfigurarComoPrueba();
            }
        }
    }
}
