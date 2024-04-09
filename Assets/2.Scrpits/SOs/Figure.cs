using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Nova Figura", menuName = "Figura")]
public class Figure : ScriptableObject
{
    public tipoDeFigura figura;
    public Sprite sprite;
    public Sprite spriteTexto;
    public string cardName;

    public bool CheckFigureIgualInteiro(int figuraEnumInt)
    {
        return ((int)figura == figuraEnumInt);
    }

    public int FigureInteiro()
    {
        return (int)figura;
    }
}

[Serializable]
public enum tipoDeFigura
{
    Null, Lava, Agua, Fogo, Terra, Ar, Mar, Onda, Calor, Vapor, Ceu, Sol, Lua, Pedra, Metal, Vento,
    Nuvem, Planta, Areia, Chuva, Alga, Concreto, Montanha, Cachoeira, Lago, Raio, Energia, Tempo, Praia, Tornado, Predio, Vida,
    Oxigenio, Arvore, Floresta, Frio, Gelo, Iglu, Casa, Cidade, Neve, Iceberg, Eletricidade, Bateria, Noite, Dia, Planeta, Gravidade,
    BuracoNegro, Galaxia, Universo, Macaco, Humano, Amor, Peixe, Tubarao, Passaro, Aguia, Inseto, Trem, Aviao, Carro, Minhoca, Ilha, Incendio, SistemaSolar,
    Madeira, Boneco, Carvao, Motor, CO2, Carbono, Petroleo, Diamante, Plastico, Circuito, Robo, Trilho, Computador, Androide, Nave, Vulcao,
    Pinguim, Navio, Cabra, Lobo, Cachorro, Lobisomem, Gato, Tigre, Manguezal, Lontra, Aranha, Teia, Tecido, Roupas, Internet, Savana,
    Alien, Tundra, Artico, Urso, Deserto, Gazela, Lagarto, Crocodilo, Dinossauro, Vidro, Aquario, Janela, Bomba, Explosao, Jato, Tanque,
    Grama, Campo, Cavalo, Zebra, Camelo, Raposa, Ferramentas, Leao, Arquipelago, UsinaEolica, Moinho, Trator, Brinquedo, VideoGame, Fazenda, UsinaHidreletrica,
    Rio, Magica, Espelho, Vaca, Continente, GuardaChuva, Papel, UrsoPolar, PainelSolar, Dragao, Mago, Castelo, Monarca, Espada, Guerreiro, Cavaleiro,
    Unicornio, Elfo, Soldado, Coroa, Ent, Golem, Mina, Livro, Fossil, Cacto, Oasis, Granizo, ArcoIris, Estante, Biblioteca, Universidade,
    Canoa, Morte, Fantasma, Ceifador, Esqueleto, Pressao, Profundezas, Sereia, Tinta, Polvo, Grafite, Canvas, Quadro, Artista, Caneta,
    PranchaDeSurfe, Caranguejo, Atmosfera, BombaNuclear, Ogro, Eclipse, Obsidiana, Gorila, Nevasca, Tempestade, Cinzas
};