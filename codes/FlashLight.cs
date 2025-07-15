using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public AudioClip interactionSound; // Som a ser tocado ao interagir
    private AudioSource audioSource; // Fonte de áudio para tocar o som

    private Light flashlight; // Componente Light
    private float range; // Variável para armazenar o range da luz

    // Start is called before the first frame update
    void Start()
    {
        flashlight = GetComponent<Light>(); // Obtém o componente Light
        range = flashlight.range; // Inicializa o range com o valor da luz

        // Cria o componente AudioSource e atribui o clip
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = interactionSound;
        audioSource.playOnAwake = false; // Desativa o play automático ao iniciar
    }

    // Update is called once per frame
    void Update()
    {
        // Verifica se o botão esquerdo do mouse foi pressionado e o range é 10
        if (Input.GetMouseButtonDown(0) && range == 10f && PauseResume.GamePaused == false)
        {
            range = 0; // Desliga a luz, definindo o range como 0
            flashlight.range = range; // Aplica o novo valor de range à luz
            audioSource.Play(); // Toca o som
        }
        // Verifica se o botão esquerdo do mouse foi pressionado e o range é 0
        else if (Input.GetMouseButtonDown(0) && range == 0 && PauseResume.GamePaused == false)
        {
            range = 10f; // Liga a luz, definindo o range como 10
            flashlight.range = range; // Aplica o novo valor de range à luz
            audioSource.Play(); // Toca o som
 }
}
}