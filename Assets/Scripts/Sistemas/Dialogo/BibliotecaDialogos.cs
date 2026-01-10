using UnityEngine;

public static class BibliotecaDialogos
{
    public static Dialogo GenerarDialogoFijo(PersonalidadNPC personalidad, bool quiereComprar)
    {
        Dialogo d = ScriptableObject.CreateInstance<Dialogo>();
        d.esVenta = quiereComprar;

        if (quiereComprar)
        {
            if (personalidad == PersonalidadNPC.Amable)
            {
                d.propuesta = "¡Hola churrero! Qué buena pinta tienen esos churros. ¿Estás vendiendo?";
                d.opciones = new string[] { "¡Hola! Sí, los mejores de la playa.", "¡Recién salidos! ¿Querés probar?", "Perdón, me queda poco." };
                d.reacciones = new string[] { "¡Genial! A ver...", "Dale, contame...", "Qué lastima, otro día será." };
            }
            else // Molesto o Indiferente
            {
                d.propuesta = "Eh, vos. ¿A cuánto tenés los churros? Espero que no me cobres como turista.";
                d.opciones = new string[] { "Están a buen precio y son de calidad.", "¿Cuántos vas a querer llevar?", "Si no te gusta, no compres." };
                d.reacciones = new string[] { "Bueno, lo dudo, pero mostrame...", "Primero decime cuánto salen...", "¡Qué maleducado!" };
            }
        }
        else
        {
            if (personalidad == PersonalidadNPC.Amable)
            {
                d.propuesta = "¡Qué lindo día para laburar! ¿Cómo te está yendo con la venta?";
                d.opciones = new string[] { "Cansado pero contento.", "Viene flojo, hay mucha competencia.", "¡Espectacular, se venden solos!" };
                d.reacciones = new string[] { "¡Esa es la actitud!", "Ya va a repuntar, ¡suerte!", "¡Me alegro mucho por vos!" };
            }
            else
            {
                d.propuesta = "Mucho calor para andar cargando ese canasto, ¿no te parece?";
                d.opciones = new string[] { "Hay que trabajar igual.", "Es parte del oficio.", "No te pregunté." };
                d.reacciones = new string[] { "Cierto, pero cuidate.", "Y bueno, cada uno con lo suyo.", "¡Qué humor!" };
            }
        }

        return d;
    }
}
