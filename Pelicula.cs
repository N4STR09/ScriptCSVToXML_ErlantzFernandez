using System;

namespace Practica2_2;
class Pelicula
{
    public string Title { get; set; }
    public string Genre { get; set; }
    public string Premiere { get; set; }
    public string Runtime { get; set; }
    public string IMDBScore { get; set; }
    public string Language { get; set; }

    public Pelicula(string title, string genre, string premiere,
                    string runtime, string imdbScore, string language)
    {
        Title = title;
        Genre = genre;
        Premiere = premiere;
        Runtime = runtime;
        IMDBScore = imdbScore;
        Language = language;
    }
}
