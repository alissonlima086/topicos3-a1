using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

public static class SeedData
{
    public static async Task CriarAdmin(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in new[] { "Admin", "Funcionario", "Usuario" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        string email = "admin@teste.com";
        string senha = "admin123";

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new Usuario
            {
                UserName = email,
                Email = email,
                Nome = "Administrador",
                Cpf = "00000000000",
                DataNascimento = new DateTime(1990, 1, 1)
            };
            var result = await userManager.CreateAsync(user, senha);
            if (!result.Succeeded)
                throw new Exception(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(user, "Admin"))
            await userManager.AddToRoleAsync(user, "Admin");
    }

    public static async Task PopularDadosTeste(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();

        if (await context.Pratos.AnyAsync()) return;

        // users
        var usuariosTeste = new[]
        {
            new { Email = "joao@teste.com",   Nome = "João Silva",     Cpf = "11122233344", Nasc = new DateTime(1992, 5, 15) },
            new { Email = "maria@teste.com",  Nome = "Maria Oliveira", Cpf = "22233344455", Nasc = new DateTime(1988, 8, 22) },
            new { Email = "carlos@teste.com", Nome = "Carlos Santos",  Cpf = "33344455566", Nasc = new DateTime(1995, 3, 10) },
        };

        var usuariosCriados = new List<Usuario>();
        foreach (var u in usuariosTeste)
        {
            var existente = await userManager.FindByEmailAsync(u.Email);
            if (existente == null)
            {
                var novo = new Usuario
                {
                    UserName = u.Email,
                    Email = u.Email,
                    Nome = u.Nome,
                    Cpf = u.Cpf,
                    DataNascimento = u.Nasc
                };
                var result = await userManager.CreateAsync(novo, "teste123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(novo, "Usuario");
                    usuariosCriados.Add(novo);
                }
            }
            else
            {
                usuariosCriados.Add(existente);
            }
        }
        // enderecos
        if (usuariosCriados.Count >= 3)
        {
            context.Enderecos.AddRange(
                new Endereco { UsuarioId = usuariosCriados[0].Id, Local = "Rua das Flores, 123",     Bairro = "Centro",        Cep = "77000-001", Complemento = "Apto 12" },
                new Endereco { UsuarioId = usuariosCriados[0].Id, Local = "Av. Palmas, 456",         Bairro = "Jardim Aureny", Cep = "77020-100", Complemento = ""        },
                new Endereco { UsuarioId = usuariosCriados[1].Id, Local = "Rua das Acácias, 78",     Bairro = "Plano Diretor", Cep = "77016-330", Complemento = "Casa"    },
                new Endereco { UsuarioId = usuariosCriados[2].Id, Local = "Quadra 304 Sul, Lote 5",  Bairro = "Plano Diretor", Cep = "77021-450", Complemento = ""        }
            );
        }

        // ingrredientes
        var ing = new[]
        {
            new Ingrediente { Nome = "Arroz",             Descricao = "Arroz branco de grão longo"   },
            new Ingrediente { Nome = "Feijão",            Descricao = "Feijão carioca"               },
            new Ingrediente { Nome = "Frango",            Descricao = "Peito de frango"              },
            new Ingrediente { Nome = "Carne Bovina",      Descricao = "Alcatra ou patinho"           },
            new Ingrediente { Nome = "Alface",            Descricao = "Alface americana"             },
            new Ingrediente { Nome = "Tomate",            Descricao = "Tomate italiano"              },
            new Ingrediente { Nome = "Cebola",            Descricao = "Cebola branca"                },
            new Ingrediente { Nome = "Alho",              Descricao = "Alho fresco"                  },
            new Ingrediente { Nome = "Manteiga",          Descricao = "Manteiga sem sal"             },
            new Ingrediente { Nome = "Queijo Minas",      Descricao = "Queijo minas frescal"         },
            new Ingrediente { Nome = "Ovo",               Descricao = "Ovo caipira"                  },
            new Ingrediente { Nome = "Macarrão",          Descricao = "Macarrão espaguete"           },
            new Ingrediente { Nome = "Molho de Tomate",   Descricao = "Molho de tomate artesanal"    },
            new Ingrediente { Nome = "Bacon",             Descricao = "Bacon defumado fatiado"       },
            new Ingrediente { Nome = "Batata",            Descricao = "Batata inglesa"               },
            new Ingrediente { Nome = "Tilápia",           Descricao = "Filé de tilápia fresco"       },
            new Ingrediente { Nome = "Limão",             Descricao = "Limão tahiti"                 },
            new Ingrediente { Nome = "Creme de Leite",    Descricao = "Creme de leite fresco"        },
            new Ingrediente { Nome = "Molho Barbecue",    Descricao = "Molho barbecue artesanal"     },
            new Ingrediente { Nome = "Costelinha",        Descricao = "Costelinha de porco"          },
            new Ingrediente { Nome = "Amendoim",          Descricao = "Amendoim torrado sem casca"   },
            new Ingrediente { Nome = "Arroz Arbóreo",     Descricao = "Arroz para risoto"            },
            new Ingrediente { Nome = "Farinha",           Descricao = "Farinha de mandioca torrada"  },
            new Ingrediente { Nome = "Pão de Hambúrguer", Descricao = "Pão brioche artesanal"        },
        };
        context.Ingredientes.AddRange(ing);
        
        // pratos
        var pratos = new[]
        {
            new Prato { Nome = "Frango Grelhado com Arroz",    Descricao = "Peito de frango grelhado com arroz branco e salada",              PrecoBase = 28.90f, Ativo = true  },
            new Prato { Nome = "Picanha na Brasa",             Descricao = "Picanha grelhada com manteiga de alho e arroz biro-biro",         PrecoBase = 59.90f, Ativo = true  },
            new Prato { Nome = "Macarrão à Bolonhesa",         Descricao = "Espaguete com molho bolonhesa de carne bovina",                   PrecoBase = 32.50f, Ativo = true  },
            new Prato { Nome = "Omelete de Queijo",            Descricao = "Omelete cremosa com queijo minas e ervas",                        PrecoBase = 18.00f, Ativo = true  },
            new Prato { Nome = "Batata Frita com Bacon",       Descricao = "Porção de batatas fritas crocantes com bacon",                    PrecoBase = 22.00f, Ativo = true  },
            new Prato { Nome = "Salada Caesar",                Descricao = "Alface americana, croutons, parmesão e molho caesar",             PrecoBase = 19.90f, Ativo = true  },
            new Prato { Nome = "Feijão Tropeiro",              Descricao = "Feijão com bacon, farinha, ovo e couve",                          PrecoBase = 25.00f, Ativo = true  },
            new Prato { Nome = "Frango à Parmegiana",          Descricao = "Frango empanado com molho de tomate e queijo gratinado",          PrecoBase = 38.90f, Ativo = true  },
            new Prato { Nome = "Estrogonofe de Frango",        Descricao = "Estrogonofe cremoso servido com arroz e batata palha",            PrecoBase = 34.00f, Ativo = true  },
            new Prato { Nome = "Bife Acebolado",               Descricao = "Bife de alcatra com cebola caramelizada e arroz",                 PrecoBase = 35.50f, Ativo = true  },
            new Prato { Nome = "Arroz com Ovo Frito",          Descricao = "Arroz branco com ovo frito na manteiga",                          PrecoBase = 14.00f, Ativo = true  },
            new Prato { Nome = "Macarrão ao Alho e Óleo",      Descricao = "Espaguete refogado no alho dourado e azeite",                     PrecoBase = 24.00f, Ativo = true  },
            new Prato { Nome = "Frango Xadrez",                Descricao = "Frango com legumes e amendoim no molho agridoce",                 PrecoBase = 36.00f, Ativo = true  },
            new Prato { Nome = "Carne Assada",                 Descricao = "Carne bovina assada lentamente com batatas",                      PrecoBase = 42.00f, Ativo = true  },
            new Prato { Nome = "Hambúrguer Artesanal",         Descricao = "Blend de carne bovina com queijo, alface e tomate",               PrecoBase = 32.00f, Ativo = true  },
            new Prato { Nome = "Panqueca de Frango",           Descricao = "Panqueca recheada com frango desfiado e molho de tomate",         PrecoBase = 27.00f, Ativo = true  },
            new Prato { Nome = "Risoto de Queijo",             Descricao = "Risoto cremoso com queijo minas e ervas finas",                   PrecoBase = 38.00f, Ativo = true  },
            new Prato { Nome = "Caldo de Feijão",              Descricao = "Caldo espesso de feijão com bacon e linguiça",                    PrecoBase = 16.00f, Ativo = true  },
            new Prato { Nome = "Tilápia Grelhada",             Descricao = "Filé de tilápia grelhado com limão e arroz",                      PrecoBase = 33.00f, Ativo = true  },
            new Prato { Nome = "Steak de Frango com Batata",   Descricao = "Steak de frango temperado com batata rústica assada",             PrecoBase = 29.00f, Ativo = true  },
            new Prato { Nome = "Costelinha ao Molho Barbecue", Descricao = "Costelinha de porco ao molho barbecue com arroz",                 PrecoBase = 49.90f, Ativo = true  },
            new Prato { Nome = "Prato do Dia (Inativo)",       Descricao = "Prato fora de temporada",                                         PrecoBase = 20.00f, Ativo = false },
        };
        context.Pratos.AddRange(pratos);

        await context.SaveChangesAsync();

        // ingredientes dos pratos
        context.PratoIngredientes.AddRange(
            // 0 - Frango Grelhado com Arroz
            new PratoIngrediente { PratoId = pratos[0].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratos[0].Id, IngredienteId = ing[0].Id  },
            new PratoIngrediente { PratoId = pratos[0].Id, IngredienteId = ing[4].Id  },
            new PratoIngrediente { PratoId = pratos[0].Id, IngredienteId = ing[7].Id  },
            // 1 - Picanha na Brasa
            new PratoIngrediente { PratoId = pratos[1].Id, IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratos[1].Id, IngredienteId = ing[7].Id  },
            new PratoIngrediente { PratoId = pratos[1].Id, IngredienteId = ing[8].Id  },
            new PratoIngrediente { PratoId = pratos[1].Id, IngredienteId = ing[0].Id  },
            // 2 - Macarrão à Bolonhesa
            new PratoIngrediente { PratoId = pratos[2].Id, IngredienteId = ing[11].Id },
            new PratoIngrediente { PratoId = pratos[2].Id, IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratos[2].Id, IngredienteId = ing[12].Id },
            new PratoIngrediente { PratoId = pratos[2].Id, IngredienteId = ing[6].Id  },
            new PratoIngrediente { PratoId = pratos[2].Id, IngredienteId = ing[7].Id  },
            // 3 - Omelete de Queijo
            new PratoIngrediente { PratoId = pratos[3].Id, IngredienteId = ing[10].Id },
            new PratoIngrediente { PratoId = pratos[3].Id, IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratos[3].Id, IngredienteId = ing[8].Id  },
            // 4 - Batata Frita com Bacon
            new PratoIngrediente { PratoId = pratos[4].Id, IngredienteId = ing[14].Id },
            new PratoIngrediente { PratoId = pratos[4].Id, IngredienteId = ing[13].Id },
            // 5 - Salada Caesar
            new PratoIngrediente { PratoId = pratos[5].Id, IngredienteId = ing[4].Id  },
            new PratoIngrediente { PratoId = pratos[5].Id, IngredienteId = ing[5].Id  },
            new PratoIngrediente { PratoId = pratos[5].Id, IngredienteId = ing[9].Id  },
            // 6 - Feijão Tropeiro
            new PratoIngrediente { PratoId = pratos[6].Id, IngredienteId = ing[1].Id  },
            new PratoIngrediente { PratoId = pratos[6].Id, IngredienteId = ing[13].Id },
            new PratoIngrediente { PratoId = pratos[6].Id, IngredienteId = ing[10].Id },
            new PratoIngrediente { PratoId = pratos[6].Id, IngredienteId = ing[22].Id },
            // 7 - Frango à Parmegiana
            new PratoIngrediente { PratoId = pratos[7].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratos[7].Id, IngredienteId = ing[12].Id },
            new PratoIngrediente { PratoId = pratos[7].Id, IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratos[7].Id, IngredienteId = ing[10].Id },
            // 8 - Estrogonofe de Frango
            new PratoIngrediente { PratoId = pratos[8].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratos[8].Id, IngredienteId = ing[17].Id },
            new PratoIngrediente { PratoId = pratos[8].Id, IngredienteId = ing[12].Id },
            new PratoIngrediente { PratoId = pratos[8].Id, IngredienteId = ing[0].Id  },
            new PratoIngrediente { PratoId = pratos[8].Id, IngredienteId = ing[14].Id },
            // 9 - Bife Acebolado
            new PratoIngrediente { PratoId = pratos[9].Id, IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratos[9].Id, IngredienteId = ing[6].Id  },
            new PratoIngrediente { PratoId = pratos[9].Id, IngredienteId = ing[0].Id  },
            new PratoIngrediente { PratoId = pratos[9].Id, IngredienteId = ing[7].Id  },
            // 10 - Arroz com Ovo Frito
            new PratoIngrediente { PratoId = pratos[10].Id, IngredienteId = ing[0].Id  },
            new PratoIngrediente { PratoId = pratos[10].Id, IngredienteId = ing[10].Id },
            new PratoIngrediente { PratoId = pratos[10].Id, IngredienteId = ing[8].Id  },
            // 11 - Macarrão ao Alho e Óleo
            new PratoIngrediente { PratoId = pratos[11].Id, IngredienteId = ing[11].Id },
            new PratoIngrediente { PratoId = pratos[11].Id, IngredienteId = ing[7].Id  },
            // 12 - Frango Xadrez
            new PratoIngrediente { PratoId = pratos[12].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratos[12].Id, IngredienteId = ing[20].Id },
            new PratoIngrediente { PratoId = pratos[12].Id, IngredienteId = ing[5].Id  },
            new PratoIngrediente { PratoId = pratos[12].Id, IngredienteId = ing[6].Id  },
            // 13 - Carne Assada
            new PratoIngrediente { PratoId = pratos[13].Id, IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratos[13].Id, IngredienteId = ing[14].Id },
            new PratoIngrediente { PratoId = pratos[13].Id, IngredienteId = ing[7].Id  },
            new PratoIngrediente { PratoId = pratos[13].Id, IngredienteId = ing[6].Id  },
            // 14 - Hambúrguer Artesanal
            new PratoIngrediente { PratoId = pratos[14].Id, IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratos[14].Id, IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratos[14].Id, IngredienteId = ing[4].Id  },
            new PratoIngrediente { PratoId = pratos[14].Id, IngredienteId = ing[5].Id  },
            new PratoIngrediente { PratoId = pratos[14].Id, IngredienteId = ing[23].Id },
            // 15 - Panqueca de Frango
            new PratoIngrediente { PratoId = pratos[15].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratos[15].Id, IngredienteId = ing[12].Id },
            new PratoIngrediente { PratoId = pratos[15].Id, IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratos[15].Id, IngredienteId = ing[10].Id },
            // 16 - Risoto de Queijo
            new PratoIngrediente { PratoId = pratos[16].Id, IngredienteId = ing[21].Id },
            new PratoIngrediente { PratoId = pratos[16].Id, IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratos[16].Id, IngredienteId = ing[8].Id  },
            new PratoIngrediente { PratoId = pratos[16].Id, IngredienteId = ing[6].Id  },
            // 17 - Caldo de Feijão
            new PratoIngrediente { PratoId = pratos[17].Id, IngredienteId = ing[1].Id  },
            new PratoIngrediente { PratoId = pratos[17].Id, IngredienteId = ing[13].Id },
            new PratoIngrediente { PratoId = pratos[17].Id, IngredienteId = ing[7].Id  },
            new PratoIngrediente { PratoId = pratos[17].Id, IngredienteId = ing[6].Id  },
            // 18 - Tilápia Grelhada
            new PratoIngrediente { PratoId = pratos[18].Id, IngredienteId = ing[15].Id },
            new PratoIngrediente { PratoId = pratos[18].Id, IngredienteId = ing[16].Id },
            new PratoIngrediente { PratoId = pratos[18].Id, IngredienteId = ing[0].Id  },
            new PratoIngrediente { PratoId = pratos[18].Id, IngredienteId = ing[7].Id  },
            // 19 - Steak de Frango com Batata
            new PratoIngrediente { PratoId = pratos[19].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratos[19].Id, IngredienteId = ing[14].Id },
            new PratoIngrediente { PratoId = pratos[19].Id, IngredienteId = ing[7].Id  },
            new PratoIngrediente { PratoId = pratos[19].Id, IngredienteId = ing[8].Id  },
            // 20 - Costelinha ao Molho Barbecue
            new PratoIngrediente { PratoId = pratos[20].Id, IngredienteId = ing[19].Id },
            new PratoIngrediente { PratoId = pratos[20].Id, IngredienteId = ing[18].Id },
            new PratoIngrediente { PratoId = pratos[20].Id, IngredienteId = ing[0].Id  }
            // 21 - Prato Inativo: sem ingredientes
        );

        // pedidos e itens
        if (usuariosCriados.Count >= 2)
        {
            var pedido1 = new Pedido
            {
                UsuarioId   = usuariosCriados[0].Id,
                DataHora    = DateTime.Now.AddDays(-2),
                Status      = WebApplication1.Models.Enums.Status.Entregue,
                TaxaEntrega = 5.00f,
                PrecoTotal  = 0f
            };
            var pedido2 = new Pedido
            {
                UsuarioId   = usuariosCriados[1].Id,
                DataHora    = DateTime.Now.AddHours(-1),
                Status      = WebApplication1.Models.Enums.Status.Pendente,
                TaxaEntrega = 0f,
                PrecoTotal  = 0f
            };

            context.Pedidos.AddRange(pedido1, pedido2);
            await context.SaveChangesAsync();

            var itens1 = new[]
            {
                new ItemPedido { PedidoId = pedido1.Id, NomePrato = pratos[0].Nome, Quantidade = 2, PrecoUnitario = pratos[0].PrecoBase, FoiSugestao = false, Observacao = "Sem alface" },
                new ItemPedido { PedidoId = pedido1.Id, NomePrato = pratos[4].Nome, Quantidade = 1, PrecoUnitario = pratos[4].PrecoBase, FoiSugestao = true,  Observacao = ""           },
            };
            var itens2 = new[]
            {
                new ItemPedido { PedidoId = pedido2.Id, NomePrato = pratos[1].Nome, Quantidade = 1, PrecoUnitario = pratos[1].PrecoBase, FoiSugestao = false, Observacao = ""           },
                new ItemPedido { PedidoId = pedido2.Id, NomePrato = pratos[2].Nome, Quantidade = 2, PrecoUnitario = pratos[2].PrecoBase, FoiSugestao = true,  Observacao = "Bem cozido" },
            };

            context.ItensPedido.AddRange(itens1);
            context.ItensPedido.AddRange(itens2);

            pedido1.PrecoTotal = itens1.Sum(i => i.PrecoUnitario * i.Quantidade) + pedido1.TaxaEntrega;
            pedido2.PrecoTotal = itens2.Sum(i => i.PrecoUnitario * i.Quantidade) + pedido2.TaxaEntrega;
        }

        await context.SaveChangesAsync();
    }
}