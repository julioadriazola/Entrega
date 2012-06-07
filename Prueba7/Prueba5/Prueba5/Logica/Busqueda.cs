using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prueba5.Models;

namespace Prueba5.Logica
{

    public class ResultadoBusqueda
    {
        public string Recorrido { get; set; }
        public int Lejania { get; set; }
        public string NombreEstado { get; set; }
        public DateTime Fecha { get; set; }
        public ResultadoBusqueda(string recorrido,int lejania,string nombreestado,DateTime fecha)
        {
            Recorrido = recorrido;
            Lejania = lejania;
            NombreEstado = nombreestado;
            Fecha = fecha;
        }
    }

    public class Busqueda
    {

        private string _paradero = null;
        private string _recorrido = null;

        private List<Paradero> _paraderos = null;
        private List<Recorrido> _recorridos = null;

        public List<Paradero> Paraderos { get { return _paraderos; } }
        public List<Recorrido> Recorridos { get { return _recorridos; } }
        public string Codigo { get { return _paradero; } }
        public string Numero { get { return _recorrido; } }

        private RecorridosParadero _recorridoParaderoIngresado = null;
        public RecorridosParadero RPIngresado { get { return _recorridoParaderoIngresado; } }

        public List<ResultadoBusqueda> ResultadoBusqueda = null;

        private static string RemoverEspacios(string s)
        {
            try
            {
                while (s.IndexOf(' ') == 0)
                    s = s.Substring(1, s.Length - 1);
                while (s.IndexOf(' ') == s.Length - 1)
                    s = s.Substring(0, s.Length - 2);
                return s;
            }
            catch
            {
                return null;
            }
        }

        private static string[] ObtenerInputs(string s)
        {
            try
            {
                string[] temp = s.Split(' ');
                List<string> result = new List<string>();
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = RemoverEspacios(temp[i]);
                    if (temp[i] != null)
                        result.Add(temp[i]);
                }
                return result.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public Busqueda(string busqueda)
        {
            string[] input = ObtenerInputs(RemoverEspacios(busqueda));
            if (input != null)
            {
                int i = 0;
                // Paramos al recorrer el arreglo o al encontrar un paradero y recorrido como inputs
                while ((i < input.Length))
                {
                    if (input[i].IndexOf('P') == 0)
                        _paradero = input[i];
                    else
                        _recorrido = input[i];
                    i++;
                }
            }
            _paraderos = ObtenerParaderos();
            _recorridos = ObtenerRecorridos();
            _recorridoParaderoIngresado = ObtenerRecorridoParadero();
            ResultadoBusqueda = ObtenerResultadoBusqueda();
        }

        private RecorridosParadero ObtenerRecorridoParadero()
        {
            if (_paradero != null & _recorrido != null)
            {
                TranSapoContext db = new TranSapoContext();
                var query = db.recorridosParadero.Where(rc => rc.Recorrido.numero == _recorrido & rc.Paradero.codigo == _paradero);
                if (query.Count<RecorridosParadero>() == 0)
                    return null;
                return query.First<RecorridosParadero>();
            }
            return null;
        }

        private List<Paradero> ObtenerParaderos()
        {
            if (_recorrido != null)
            {
                TranSapoContext db = new TranSapoContext();

                var query = from rp in db.recorridosParadero.OrderBy(r => r.NumeroParada)
                            where rp.Recorrido.numero==_recorrido
                            select rp.Paradero;
                
                if (query.Count<Paradero>() == 0)
                    return null;
                return query.OfType<Paradero>().ToList<Paradero>();
            }
            return null;
        }

        private List<Recorrido> ObtenerRecorridos()
        {
            if (_paradero != null)
            {
                TranSapoContext db = new TranSapoContext();

                var query = from rp in db.recorridosParadero
                            where rp.Paradero.codigo == _paradero
                            select rp.Recorrido;

                if (query.Count<Recorrido>() == 0)
                    return null;


                return query.OfType<Recorrido>().ToList<Recorrido>();
            }
            return null;
        }

        public bool ExisteResultadoFinal()
        {
            return ResultadoBusqueda != null;
        }

        private List<ResultadoBusqueda> ObtenerResultadoBusqueda()
        {
            if (this.RPIngresado==null) // condicion de que no exista
                return null;
            TranSapoContext db = new Prueba5.Models.TranSapoContext();
            var informaciones = db.Informaciones.Where(i => i.Recorrido.numero == this.Numero).ToList<Prueba5.Models.Informacion>();
            var recorridoparadero = db.recorridosParadero.Where(r => r.Recorrido.numero == this.Numero & r.NumeroParada < this.RPIngresado.NumeroParada).ToList<Prueba5.Models.RecorridosParadero>();
            var query = informaciones.AsQueryable().Join(recorridoparadero, i => i.Paradero, rc => rc.Paradero, (i, rc) => new { Recorrido = i.Recorrido.numero, Fecha = i.fecha, NombreEstado = i.Estado.NombreEstado, Lejania=this.RPIngresado.NumeroParada-rc.NumeroParada }).OrderBy(a=>a.Lejania);

                List<ResultadoBusqueda> resultado = new List<ResultadoBusqueda>();
                foreach (var q in query)
                    resultado.Add(new ResultadoBusqueda(q.Recorrido, q.Lejania, q.NombreEstado, q.Fecha));
                return resultado;
        }

    }
}