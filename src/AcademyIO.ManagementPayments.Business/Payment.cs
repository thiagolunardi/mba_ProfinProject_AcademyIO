﻿using AcademyIO.Core.DomainObjects;

namespace AcademyIO.ManagementPayments.Business;

public class Payment : Entity, IAggregateRoot
{
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }
    public double Value { get; set; }
    public string CardName { get; set; }
    public string CardNumber { get; set; }
    public string CardExpirationDate { get; set; }
    public string CardCVV { get; set; }
    public BusinessTransaction Transaction { get; set; }
}