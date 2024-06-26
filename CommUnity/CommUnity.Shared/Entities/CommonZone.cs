﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommUnity.Shared.Entities
{
    public class CommonZone
    {
        public int Id { get; set; }

        [Display(Name = "Zona Común")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!;

        public int Capacity { get; set; }

        public int ResidentialUnitId { get; set; }

        public ResidentialUnit? ResidentialUnit { get; set; }

        public ICollection<CommonZoneReservation>? Reservations { get; set; }

        [Display(Name = "Eventos")]
        public int EventsNumber => Reservations == null || Reservations.Count == 0 ? 0 : Reservations.Count;
    }
}