using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManejadorJuego : MonoBehaviour {

    [Header("Partida")]
    public bool turnoJugador = true;
    public int ronda = 1;

    [Header("Mazos")]
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

    }

    void Start() {

    }

    void Update() {

    }

    #region Inicial

    public void AgregarCamellosIniciales() {
       
    }

    public void LlenarMercado() {

    }

    public void RepartirAJugadores() {

    }

    public void OrdenarFichas() {

    }

    public void DarAJugador(GameObject carta) {

    }

    public void DarAOponente(GameObject carta) {

    }

    #endregion

    #region Selección


    public bool AgregarASeleccion(GameObject objeto, bool checarSeleccion) {
        return true;
    }

    public bool RemoverDeSeleccion(GameObject objeto, bool checarSeleccion) {
        return true;
    }

    public void LimpiarSeleccion() {

    }

    public void LimpiarSeleccion(Grupo grupo) {

    }

    public void ActualizarReferenciasCartas() {

    }

    bool seleccionadosTodosCamellosMercado = false;

    public void SeleccionarCamellosMercado(bool seleccion) {

    }

    #endregion

    #region Acciones del Jugador

    public void TomarUna() {

    }

    public void Vender() {

    }

    public void Trueque() {

    }

    #endregion

    #region Chequeo

    bool PuedeAceptar(int cartasNormales, int camellos) {
        return true;
    }

    #endregion

    #region Interfaz

    void EstadoPanelAcciones(bool estado) {
       
    }

    #endregion

    #region Corrutinas

    IEnumerator Juego() {
        yield return new WaitForEndOfFrame();
    }

    IEnumerator RepartirAJugadoresAnimado() {
        yield return new WaitForEndOfFrame();
    }

    IEnumerator LlenarMercadoAnimado() {
        yield return new WaitForEndOfFrame();
    }

    IEnumerator OrdenarFichasAnimado() {
        yield return new WaitForEndOfFrame();
    }

    #endregion

    #region Multijugador

    List<Carta> cartasMovidas = new List<Carta>();

    public void EjecutarMovimientoOponente(Movimiento movimiento) {

    }

    public void MandarMovimientoJugador(Movimiento movimiento) {

    }

    #endregion
}
