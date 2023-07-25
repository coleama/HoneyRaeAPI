using HoneyRaeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;

List<Customer> customers = new List<Customer>
{
    new Customer {Id = 1, Name = "Snek Plisken", Address = "1111 Home Street"},
    new Customer {Id = 2, Name = "Soild Snek", Address = "1112 NotHome Street"},
    new Customer {Id = 3, Name = "NotSoild Snek", Address = "1113 Far Street"}
};
List<Employee> employees = new List<Employee>
{
    new Employee {Id = 1, Name = "Doctor Small Balls", Specialty = "Andrologists" },
    new Employee {Id = 2, Name = "Ted", Specialty = "Stuff"},
    new Employee {Id = 3, Name = "Not Ted", Specialty = "Not Stuff"}
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket>
    {   
    new ServiceTicket
            {
                Id = 1,
                CustomerId = 1,
                EmployeeId = null,
                Description = "Ticket #1: The mysterious case of the dancing laptop",
                Emergency = false,
                DateCompleted = DateTime.Now
            },
            new ServiceTicket
            {
                Id = 2,
                CustomerId = 2,
                EmployeeId = 3,
                Description = "Ticket #2: The rebellious printer that only prints cat memes",
                Emergency = true,
                DateCompleted = DateTime.Now
            },
            new ServiceTicket
            {
                Id = 3,
                CustomerId = 1,
                EmployeeId = 2,
                Description = "Ticket #3: Installing 'Infinite Loop' software - hope it doesn't crash the universe",
                Emergency = false,
                DateCompleted = DateTime.Now
            },
            new ServiceTicket
            {
                Id = 4,
                CustomerId = 3,
                EmployeeId = 2,
                Description = "Ticket #4: Giving a jetpack to the office chair - productivity booster!",
                Emergency = false,
                DateCompleted = DateTime.Now
            },
            new ServiceTicket
            {
                Id = 5,
                CustomerId = 3,
                EmployeeId = null,
                Description = "Ticket #5: The toaster that's overachieving as a smoke machine",
                Emergency = true,
                DateCompleted = null
            }
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

   //Service tickets
app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
    return Results.Ok(serviceTicket);
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    serviceTickets.Remove(serviceTicket);
    return (serviceTicket);
});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    //the id in the request route doesn't match the id from the ticket in the request body. That's a bad request!
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();
});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
});

//Customers
app.MapGet("/customers", () =>
{
    return customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(st => st.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return Results.Ok(customer);
});

//Employees
app.MapGet("/employees", () =>
{
    return employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
