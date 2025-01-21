using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Repository.Entities;
using MongoDB.Bson;
using Portfolio.Command;
using Products.Command;
using Products.Event;
using Statement.Event;

namespace Investments.Mapp
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            //products
            CreateMap<ProductDomain, CreateProductCommand>();
            CreateMap<CreateProductCommand, ProductDomain>();
            CreateMap<ProductDomain, CreateProductEvent>();
            CreateMap<CreateProductEvent, ProductDomain>();
            CreateMap<CreateProductCommand, CreateProductEvent>();
            CreateMap<CreateProductEvent, CreateProductCommand>();


            CreateMap<ProductDomain, UpdateProductCommand>();
            CreateMap<UpdateProductCommand, ProductDomain>();
            CreateMap<ProductDomain, UpdateProductEvent>();
            CreateMap<UpdateProductEvent, ProductDomain>();
            CreateMap<UpdateProductCommand, UpdateProductEvent>();
            CreateMap<UpdateProductEvent, UpdateProductCommand>();


            CreateMap<UserDomain, Users.Command.UpdateUserCommand>();
            CreateMap<Users.Command.UpdateUserCommand, UserDomain > ();
            CreateMap<UserDomain, Users.Command.CreateUserCommand>();
            CreateMap<Users.Command.CreateUserCommand, UserDomain>();


            CreateMap<CustomerDomain, Customers.Command.UpdateCustomerCommand>();
            CreateMap<Customers.Command.UpdateCustomerCommand, CustomerDomain>();
            CreateMap<CustomerDomain, Customers.Command.CreateCustomerCommand>();
            CreateMap<Customers.Command.CreateCustomerCommand, CustomerDomain>();


            CreateMap<PortfolioRequest, OperatePortfolioCustomerCommand>();
            CreateMap<OperatePortfolioCustomerCommand, PortfolioRequest>();
            CreateMap<OperatePortfolioCustomerCommand, InsertPortfolioStatementByCustomerEvent>();
            CreateMap<InsertPortfolioStatementByCustomerEvent, OperatePortfolioCustomerCommand>();

            


        }
    }
}
