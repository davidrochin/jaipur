using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grupo : MonoBehaviour {
    [Header("Ajustes")]
    public TipoAcomodo tipoAcomodo = TipoAcomodo.Vertical;
    public TipoMazo tipoMazo = TipoMazo.Cartas;
    public bool objetosSeleccionables = false;

    [Header("Estetica")]
    public bool objetosCaraAbajo = false;
    public bool rotacionDesordenada = false;
    public float maxRotacionAleatoria = 7f;
    public bool invertirDireccionLateral = false;
    float inversorDireccion = 1f;

    //Arreglo que contiene los hijos de este objeto (osea las cartas)
    public GameObject[] hijos;
    public Carta[] hijosCarta;
    bool autoActualizarHijos = true;

    public bool congelado = false;

    private float separacionVertical = 0.01f;
    private float separacionLateral = 1f;

    void Awake() {

    }

    void Update() {

    }

    private void OnTransformChildrenChanged() {

    }

    GameObject[] ObtenerHijos() {
        return null;
    }

    Vector3 ObtenerPosicionEnMazo(int indiceEnMazo) {
        return new Vector3();
    }

    Quaternion ObtenerRotacionEnMazo(int indiceEnMazo) {
        return new Quaternion();
    }

    void CalcularSeparaciones() {

    }

    #region Acciones

    public GameObject[] ObtenerUltimasCartas(int cantidad) {
        return null;
    }

    public GameObject ObtenerUltimaCarta() {
        return null;
    }

    public Carta[] RevolverCartas() {
        return null;
    }

    public void OrdenarPorTipo() {

    }

    public GameObject[] ObtenerCamellos(int cantidad) {
        return null;
    }

    #endregion

    private void OnDrawGizmos() {

    }

    /*Este es un procedimiento para dibujar un Cubo que soporta rotación. Fue sacado
    de Unity Forums.*/
    public void DrawCube(Vector3 position, Quaternion rotation, Vector3 scale) {
        Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, scale);
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        Gizmos.matrix *= cubeTransform;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldGizmosMatrix;
    }

    public enum TipoAcomodo { Vertical, Lateral, LateralEncimado, Mano }
    public enum TipoMazo { Cartas, Fichas }
}
