using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreadorDialogos : MonoBehaviour {

    public static CreadorDialogos singleton;

    public GameObject prefabDialogo;

    private void Awake() {
        if(singleton == null) {
            singleton = this;
        } else if(singleton != null && singleton != this) {
            Destroy(gameObject);
        }
    }

    public void CrearDialogo(string mensaje, AccionBoton accion){

        //Instanciar el dialogo y hacerlo el ultimo hijo del Canvas
        GameObject objetoDialogo = Instantiate(prefabDialogo, FindObjectOfType<Canvas>().transform, false);
        objetoDialogo.transform.SetAsLastSibling();

        //Buscar el texto del dialogo y cambiarlo
        foreach (Transform hijo in objetoDialogo.transform) { if (hijo.GetComponent<Text>() != null) { hijo.GetComponent<Text>().text = mensaje; } }

        //Establecer la acción del botón de aceptar
        Button boton = objetoDialogo.GetComponentInChildren<Button>();
        switch (accion) {
            case AccionBoton.Nada:
                boton.onClick.AddListener(delegate { AccionBotonNada(objetoDialogo); });
                break;
            case AccionBoton.SalirAlMenu:
                boton.onClick.AddListener(delegate { AccionBotonSalirAlMenu(objetoDialogo); });
                break;
            default:
                boton.onClick.AddListener(delegate { AccionBotonNada(objetoDialogo); });
                break;
        }
        
    }

    public void AccionBotonNada(GameObject llamador) {
        Destroy(llamador);
    }

    public void AccionBotonSalirAlMenu(GameObject llamador) {
        FindObjectOfType<ManejadorRed>().CerrarTodo();
        FindObjectOfType<ManejadorMenu>().CargarEscena("menu");
    }

    public enum AccionBoton { Nada, SalirAlMenu }
}
