using UnityEngine;
using System.Collections.Generic;

public static class BibliotecaDialogos
{
    public static Dialogo GenerarDialogoAleatorio(PersonalidadNPC personalidad, bool quiereComprar)
    {
        Dialogo d = ScriptableObject.CreateInstance<Dialogo>();
        d.esVenta = quiereComprar;

        if (quiereComprar)
        {
            ConfigurarVenta(d, personalidad);
        }
        else
        {
            ConfigurarSocial(d, personalidad);
        }

        return d;
    }

    private static void ConfigurarVenta(Dialogo d, PersonalidadNPC p)
    {
        if (p == PersonalidadNPC.Amable)
        {
            string[] propuestas = {
                "¡Esa, churrero! ¿Tenes de los buenos?",
                "¡Justo lo que buscaba! Vení para acá.",
                "¡Qué olorcito! Me convenciste, ¿están frescos?",
                "¡Genio! Salvame la tarde con unos churros.",
                "¡Eh, amigo! ¿De qué tenés? Dame de los mejores.",
                "¡Vení crack! Mi vieja quiere unos churros."
            };
            d.propuesta = propuestas[Random.Range(0, propuestas.Length)];
            d.opciones = new string[] { "¡Obvio, los mejores!", "Claro, ¿cuántos quieres?", "Recién salidos." };
            d.reacciones = new string[] { "¡A ver, mostrame!", "Dale, me sirven.", "Si es así, dame." };
        }
        else if (p == PersonalidadNPC.Molesto)
        {
            string[] propuestas = {
                "Bueno, bueno, no grités tanto. ¿A cuánto los tenés?",
                "Si no están aceitosos te compro algo...",
                "A ver... ¿valen la pena tus churros?",
                "Vení, apurate que me quiero ir al agua.",
                "¡Uf! Qué caro está todo. ¿Me hacés precio?",
                "Espero que no tengan arena..."
            };
            d.propuesta = propuestas[Random.Range(0, propuestas.Length)];
            d.opciones = new string[] { "Son de primera calidad.", "Te van a encantar.", "Están secos y crocantes." };
            d.reacciones = new string[] { "Mmm, ya veremos.", "Bueno, pero no me fajes con el precio.", "A ver, mostrame." };
        }
        else // Indiferente o resto
        {
            d.propuesta = "Eh... ¿vendés churros? ¿A cuánto?";
            d.opciones = new string[] { "¡Sí! Muy económicos.", "De dulce de leche y crema.", "¡Llevale a toda la familia!" };
            d.reacciones = new string[] { "Bueno, dame algunos.", "Dejame ver...", "Y dale..." };
        }
    }

    private static void ConfigurarSocial(Dialogo d, PersonalidadNPC p)
    {
        if (p == PersonalidadNPC.Amable)
        {
            string[] propuestas = {
                "¿Cómo viene la venta, jefe? Está brava la arena hoy, ¿no?",
                "¡Qué laburo el tuyo, eh! Bajo el sol todo el día.",
                "¡Esa, churrero! Te veo en todas partes, sos un laburante."
            };
            d.propuesta = propuestas[Random.Range(0, propuestas.Length)];
            d.opciones = new string[] { "Se hace lo que se puede.", "Uf, el sol mata.", "¡A full por suerte!" };
            d.reacciones = new string[] { "¡Suerte con eso!", "¡Tomá mucha agua, che!", "¡Esa es la actitud!" };
            d.impactos = new Consecuencia[] { 
                new Consecuencia { reputacion = 1, hidratacion = 2 },
                new Consecuencia { reputacion = 1, stamina = 5 },
                new Consecuencia { reputacion = 2, stamina = 10 }
            };
        }
        else if (p == PersonalidadNPC.Molesto)
        {
            string[] propuestas = {
                "¡Eh! Cuidado donde pisás que me llenás de arena la lona.",
                "No podés pasar por otro lado? Estoy tratando de dormir.",
                "¿A qué hora terminás de gritar? Es domingo, flaco."
            };
            d.propuesta = propuestas[Random.Range(0, propuestas.Length)];
            d.opciones = new string[] { "Perdón, no te vi.", "La playa es de todos.", "Correte un poquito entonces." };
            d.reacciones = new string[] { "Mirá por donde vas la próxima.", "¡Qué atrevido!", "¡Encima maleducado!" };
            d.impactos = new Consecuencia[] { 
                new Consecuencia { reputacion = 1 },
                new Consecuencia { reputacion = -2, stamina = -5 },
                new Consecuencia { reputacion = -5, stamina = -10 }
            };
        }
        else
        {
            d.propuesta = "Lindo día hoy, ¿no?";
            d.opciones = new string[] { "Ideal para un churro.", "Demasiado calor.", "Sí, impecable." };
            d.reacciones = new string[] { "Y sí...", "Puede ser.", "Tal cual." };
        }
    }
}
