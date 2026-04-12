using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Enums;

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

        // ── Usuários de teste ────────────────────────────────────────────────
        var usuariosTeste = new[]
        {
            new { Email = "joao@teste.com",   Nome = "João Silva",       Cpf = "11122233344", Nasc = new DateTime(1992,  5, 15) },
            new { Email = "maria@teste.com",  Nome = "Maria Oliveira",   Cpf = "22233344455", Nasc = new DateTime(1988,  8, 22) },
            new { Email = "carlos@teste.com", Nome = "Carlos Santos",    Cpf = "33344455566", Nasc = new DateTime(1995,  3, 10) },
            new { Email = "ana@teste.com",    Nome = "Ana Costa",        Cpf = "44455566677", Nasc = new DateTime(2000,  7,  5) },
            new { Email = "pedro@teste.com",  Nome = "Pedro Almeida",    Cpf = "55566677788", Nasc = new DateTime(1985, 11, 20) },
        };

        var usuarios = new List<Usuario>();
        foreach (var u in usuariosTeste)
        {
            var existente = await userManager.FindByEmailAsync(u.Email);
            if (existente == null)
            {
                var novo = new Usuario
                {
                    UserName = u.Email, Email = u.Email,
                    Nome = u.Nome, Cpf = u.Cpf, DataNascimento = u.Nasc
                };
                var result = await userManager.CreateAsync(novo, "Teste@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(novo, "Usuario");
                    usuarios.Add(novo);
                }
            }
            else { usuarios.Add(existente); }
        }

        // ── Endereços ────────────────────────────────────────────────────────
        if (usuarios.Count >= 3)
        {
            context.Enderecos.AddRange(
                new Endereco { UsuarioId = usuarios[0].Id, Local = "Rua das Flores, 123",    Bairro = "Centro",         Cep = "77000-001", Complemento = "Apto 12" },
                new Endereco { UsuarioId = usuarios[0].Id, Local = "Av. Palmas, 456",        Bairro = "Jardim Aureny",  Cep = "77020-100", Complemento = ""        },
                new Endereco { UsuarioId = usuarios[1].Id, Local = "Rua das Acácias, 78",    Bairro = "Plano Diretor",  Cep = "77016-330", Complemento = "Casa"    },
                new Endereco { UsuarioId = usuarios[2].Id, Local = "Quadra 304 Sul, Lote 5", Bairro = "Plano Diretor",  Cep = "77021-450", Complemento = ""        },
                new Endereco { UsuarioId = usuarios[3].Id, Local = "Rua 02, Q 07, Lt 10",   Bairro = "Jardim Taquari", Cep = "77025-062", Complemento = "Bloco B" }
            );
        }

        // ── Mesas ────────────────────────────────────────────────────────────
        var mesas = new[]
        {
            new Mesa { Numero = 1,  Capacidade = 2,  Disponivel = true  },
            new Mesa { Numero = 2,  Capacidade = 2,  Disponivel = true  },
            new Mesa { Numero = 3,  Capacidade = 4,  Disponivel = true  },
            new Mesa { Numero = 4,  Capacidade = 4,  Disponivel = true  },
            new Mesa { Numero = 5,  Capacidade = 4,  Disponivel = true  },
            new Mesa { Numero = 6,  Capacidade = 6,  Disponivel = true  },
            new Mesa { Numero = 7,  Capacidade = 6,  Disponivel = true  },
            new Mesa { Numero = 8,  Capacidade = 8,  Disponivel = true  },
            new Mesa { Numero = 9,  Capacidade = 8,  Disponivel = true  },
            new Mesa { Numero = 10, Capacidade = 10, Disponivel = true  },
        };
        context.Mesas.AddRange(mesas);

        // ── Configurações de Delivery ─────────────────────────────────────────
        var configDelivery = new[]
        {
            new ConfiguracaoDelivery
            {
                Tipo = TipoDelivery.Proprio,
                NomeApp = "Entrega Própria",
                TaxaFixaProprio = 5.00f,
                Ativo = true
            },
            new ConfiguracaoDelivery
            {
                Tipo = TipoDelivery.App,
                NomeApp = "iFood",
                ComissaoPorcentagem = 12,
                TaxaAdicionalApp = 2.50f,
                Ativo = true
            },
            new ConfiguracaoDelivery
            {
                Tipo = TipoDelivery.App,
                NomeApp = "Rappi",
                ComissaoPorcentagem = 15,
                TaxaAdicionalApp = 3.00f,
                Ativo = false
            },
        };
        context.ConfiguracoesDelivery.AddRange(configDelivery);

        // ── Ingredientes ──────────────────────────────────────────────────────
        var ing = new[]
        {
            /* 00 */ new Ingrediente { Nome = "Arroz",              Descricao = "Arroz branco de grão longo"          },
            /* 01 */ new Ingrediente { Nome = "Feijão",             Descricao = "Feijão carioca"                      },
            /* 02 */ new Ingrediente { Nome = "Frango",             Descricao = "Peito de frango"                     },
            /* 03 */ new Ingrediente { Nome = "Carne Bovina",       Descricao = "Alcatra ou patinho"                  },
            /* 04 */ new Ingrediente { Nome = "Alface",             Descricao = "Alface americana"                    },
            /* 05 */ new Ingrediente { Nome = "Tomate",             Descricao = "Tomate italiano"                     },
            /* 06 */ new Ingrediente { Nome = "Cebola",             Descricao = "Cebola branca"                       },
            /* 07 */ new Ingrediente { Nome = "Alho",               Descricao = "Alho fresco"                         },
            /* 08 */ new Ingrediente { Nome = "Manteiga",           Descricao = "Manteiga sem sal"                    },
            /* 09 */ new Ingrediente { Nome = "Queijo Minas",       Descricao = "Queijo minas frescal"                },
            /* 10 */ new Ingrediente { Nome = "Ovo",                Descricao = "Ovo caipira"                         },
            /* 11 */ new Ingrediente { Nome = "Macarrão",           Descricao = "Macarrão espaguete"                  },
            /* 12 */ new Ingrediente { Nome = "Molho de Tomate",    Descricao = "Molho de tomate artesanal"           },
            /* 13 */ new Ingrediente { Nome = "Bacon",              Descricao = "Bacon defumado fatiado"              },
            /* 14 */ new Ingrediente { Nome = "Batata",             Descricao = "Batata inglesa"                      },
            /* 15 */ new Ingrediente { Nome = "Tilápia",            Descricao = "Filé de tilápia fresco"              },
            /* 16 */ new Ingrediente { Nome = "Limão",              Descricao = "Limão tahiti"                        },
            /* 17 */ new Ingrediente { Nome = "Creme de Leite",     Descricao = "Creme de leite fresco"               },
            /* 18 */ new Ingrediente { Nome = "Molho Barbecue",     Descricao = "Molho barbecue artesanal"            },
            /* 19 */ new Ingrediente { Nome = "Costelinha",         Descricao = "Costelinha de porco"                 },
            /* 20 */ new Ingrediente { Nome = "Amendoim",           Descricao = "Amendoim torrado sem casca"          },
            /* 21 */ new Ingrediente { Nome = "Arroz Arbóreo",      Descricao = "Arroz para risoto"                   },
            /* 22 */ new Ingrediente { Nome = "Farinha de Mandioca",Descricao = "Farinha de mandioca torrada"         },
            /* 23 */ new Ingrediente { Nome = "Pão de Hambúrguer",  Descricao = "Pão brioche artesanal"               },
            /* 24 */ new Ingrediente { Nome = "Camarão",            Descricao = "Camarão VG limpo"                    },
            /* 25 */ new Ingrediente { Nome = "Azeite",             Descricao = "Azeite extravirgem"                  },
            /* 26 */ new Ingrediente { Nome = "Pimentão",           Descricao = "Pimentão vermelho e amarelo"         },
            /* 27 */ new Ingrediente { Nome = "Couve",              Descricao = "Couve manteiga fatiada"              },
            /* 28 */ new Ingrediente { Nome = "Linguiça",           Descricao = "Linguiça toscana"                    },
            /* 29 */ new Ingrediente { Nome = "Queijo Parmesão",    Descricao = "Parmesão ralado"                     },
            /* 30 */ new Ingrediente { Nome = "Cogumelo",           Descricao = "Cogumelo paris fatiado"              },
            /* 31 */ new Ingrediente { Nome = "Espinafre",          Descricao = "Espinafre fresco"                    },
            /* 32 */ new Ingrediente { Nome = "Salmão",             Descricao = "Filé de salmão fresco"               },
            /* 33 */ new Ingrediente { Nome = "Alcaparra",          Descricao = "Alcaparra em conserva"               },
            /* 34 */ new Ingrediente { Nome = "Pepino",             Descricao = "Pepino japonês"                      },
            /* 35 */ new Ingrediente { Nome = "Cenoura",            Descricao = "Cenoura fresca"                      },
            /* 36 */ new Ingrediente { Nome = "Ervilha",            Descricao = "Ervilha fresca ou congelada"         },
            /* 37 */ new Ingrediente { Nome = "Pão Italiano",       Descricao = "Pão italiano artesanal"              },
            /* 38 */ new Ingrediente { Nome = "Mostarda Dijon",     Descricao = "Mostarda francesa"                   },
            /* 39 */ new Ingrediente { Nome = "Mel",                Descricao = "Mel puro de abelha"                  },
        };
        context.Ingredientes.AddRange(ing);

        // ── Pratos de ALMOÇO (25) ─────────────────────────────────────────────
        var pratosAlmoco = new[]
        {
            /* A00 */ new Prato { Nome = "Frango Grelhado com Arroz",      Descricao = "Peito de frango grelhado com arroz e salada verde",              PrecoBase = 28.90f, Ativo = true,  Turno = Turno.Almoco },
            /* A01 */ new Prato { Nome = "Feijão Tropeiro Mineiro",        Descricao = "Feijão com bacon, farinha, ovo, couve e linguiça",               PrecoBase = 25.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A02 */ new Prato { Nome = "Macarrão à Bolonhesa",           Descricao = "Espaguete com molho bolonhesa de carne bovina",                  PrecoBase = 32.50f, Ativo = true,  Turno = Turno.Almoco },
            /* A03 */ new Prato { Nome = "Frango Xadrez",                  Descricao = "Frango com legumes e amendoim no molho agridoce",                PrecoBase = 36.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A04 */ new Prato { Nome = "Bife Acebolado",                 Descricao = "Bife de alcatra com cebola caramelizada e arroz",                PrecoBase = 35.50f, Ativo = true,  Turno = Turno.Almoco },
            /* A05 */ new Prato { Nome = "Arroz com Ovo Frito",            Descricao = "Arroz branco com ovo frito na manteiga e alho",                  PrecoBase = 14.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A06 */ new Prato { Nome = "Frango à Parmegiana",            Descricao = "Frango empanado com molho de tomate e queijo gratinado",         PrecoBase = 38.90f, Ativo = true,  Turno = Turno.Almoco },
            /* A07 */ new Prato { Nome = "Tilápia Grelhada com Arroz",     Descricao = "Filé de tilápia grelhado com limão e arroz branco",              PrecoBase = 33.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A08 */ new Prato { Nome = "Estrogonofe de Frango",          Descricao = "Estrogonofe cremoso servido com arroz e batata palha",           PrecoBase = 34.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A09 */ new Prato { Nome = "Carne Assada com Legumes",       Descricao = "Carne bovina assada lentamente com cenoura e batata",            PrecoBase = 42.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A10 */ new Prato { Nome = "Panqueca de Frango",             Descricao = "Panqueca recheada com frango desfiado e molho de tomate",        PrecoBase = 27.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A11 */ new Prato { Nome = "Macarrão ao Alho e Óleo",        Descricao = "Espaguete refogado no alho dourado e azeite",                   PrecoBase = 24.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A12 */ new Prato { Nome = "Arroz com Feijão e Bife",        Descricao = "PF tradicional com bife grelhado, arroz e feijão",               PrecoBase = 22.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A13 */ new Prato { Nome = "Frango ao Molho de Cogumelos",   Descricao = "Frango grelhado com molho cremoso de cogumelos",                 PrecoBase = 39.90f, Ativo = true,  Turno = Turno.Almoco },
            /* A14 */ new Prato { Nome = "Omelete de Queijo e Espinafre",  Descricao = "Omelete cremosa com queijo minas e espinafre refogado",          PrecoBase = 19.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A15 */ new Prato { Nome = "Steak de Frango com Batata",     Descricao = "Steak de frango temperado com batata rústica assada",            PrecoBase = 29.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A16 */ new Prato { Nome = "Linguiça Acebolada com Arroz",   Descricao = "Linguiça toscana grelhada com cebola e arroz",                   PrecoBase = 23.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A17 */ new Prato { Nome = "Risoto de Queijo e Ervilha",     Descricao = "Risoto cremoso com queijo parmesão e ervilhas frescas",          PrecoBase = 38.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A18 */ new Prato { Nome = "Salada Caesar com Frango",       Descricao = "Alface romana, frango grelhado, croutons e molho caesar",        PrecoBase = 26.90f, Ativo = true,  Turno = Turno.Almoco },
            /* A19 */ new Prato { Nome = "Frango Ensopado com Batata",     Descricao = "Frango cozido no caldo com batatas e cenoura",                   PrecoBase = 31.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A20 */ new Prato { Nome = "Macarrão com Frango e Requeijão",Descricao = "Macarrão cremoso com frango desfiado e requeijão",               PrecoBase = 30.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A21 */ new Prato { Nome = "Batata Recheada com Frango",     Descricao = "Batata assada recheada com frango cremoso e queijo gratinado",   PrecoBase = 27.50f, Ativo = true,  Turno = Turno.Almoco },
            /* A22 */ new Prato { Nome = "Espaguete ao Molho Branco",      Descricao = "Espaguete com molho branco cremoso e queijo parmesão",           PrecoBase = 29.90f, Ativo = true,  Turno = Turno.Almoco },
            /* A23 */ new Prato { Nome = "Caldo de Feijão com Bacon",      Descricao = "Caldo espesso de feijão com bacon, alho e temperos",             PrecoBase = 16.00f, Ativo = true,  Turno = Turno.Almoco },
            /* A24 */ new Prato { Nome = "Frango Caipira Refogado",        Descricao = "Frango caipira refogado com cebola, alho e tomate",              PrecoBase = 34.50f, Ativo = true,  Turno = Turno.Almoco },
        };
        context.Pratos.AddRange(pratosAlmoco);

        // ── Pratos de JANTAR (25) ─────────────────────────────────────────────
        var pratosJantar = new[]
        {
            /* J00 */ new Prato { Nome = "Picanha na Brasa",               Descricao = "Picanha grelhada com manteiga de alho e arroz biro-biro",        PrecoBase = 59.90f, Ativo = true,  Turno = Turno.Jantar },
            /* J01 */ new Prato { Nome = "Costelinha ao Molho Barbecue",   Descricao = "Costelinha de porco ao molho barbecue defumado",                  PrecoBase = 49.90f, Ativo = true,  Turno = Turno.Jantar },
            /* J02 */ new Prato { Nome = "Hambúrguer Artesanal",           Descricao = "Blend de carne bovina com queijo, alface e tomate no brioche",    PrecoBase = 32.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J03 */ new Prato { Nome = "Salmão Grelhado",                Descricao = "Filé de salmão grelhado com alcaparras e limão",                 PrecoBase = 62.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J04 */ new Prato { Nome = "Camarão ao Alho e Óleo",         Descricao = "Camarão VG salteado no azeite e alho com arroz",                 PrecoBase = 68.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J05 */ new Prato { Nome = "Frango Assado Inteiro",          Descricao = "Frango assado temperado com ervas e acompanhamentos",             PrecoBase = 55.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J06 */ new Prato { Nome = "Risoto de Camarão",              Descricao = "Risoto cremoso de camarão com toque de limão siciliano",          PrecoBase = 72.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J07 */ new Prato { Nome = "Filé Mignon ao Molho Madeira",   Descricao = "Medalhão de filé ao molho madeira com batata dauphine",          PrecoBase = 79.90f, Ativo = true,  Turno = Turno.Jantar },
            /* J08 */ new Prato { Nome = "Batata Frita com Bacon",         Descricao = "Porção de batatas fritas crocantes com bacon e queijo",           PrecoBase = 22.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J09 */ new Prato { Nome = "Alcatra ao Molho de Cogumelos",  Descricao = "Bife de alcatra grelhado com molho rústico de cogumelos",         PrecoBase = 52.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J10 */ new Prato { Nome = "Penne ao Molho Rosé",            Descricao = "Penne com molho rosé cremoso, presunto e champignon",             PrecoBase = 36.90f, Ativo = true,  Turno = Turno.Jantar },
            /* J11 */ new Prato { Nome = "Carne de Sol na Chapa",          Descricao = "Carne de sol grelhada com arroz, feijão e manteiga",              PrecoBase = 44.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J12 */ new Prato { Nome = "Frango Grelhado com Mel e Mostarda", Descricao = "Frango ao molho de mel e mostarda dijon com legumes",         PrecoBase = 41.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J13 */ new Prato { Nome = "Espetinho Misto",                Descricao = "Espeto com carne bovina, frango e linguiça com vinagrete",        PrecoBase = 38.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J14 */ new Prato { Nome = "Tilápia ao Molho de Camarão",   Descricao = "Filé de tilápia grelhado coberto com molho de camarão",           PrecoBase = 54.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J15 */ new Prato { Nome = "Macarrão com Frutos do Mar",     Descricao = "Espaguete com camarão, tilápia e molho de tomate fresco",         PrecoBase = 58.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J16 */ new Prato { Nome = "Porção de Frango Frito",         Descricao = "Pedaços de frango empanado e frito com molho especial",           PrecoBase = 35.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J17 */ new Prato { Nome = "Risoto de Salmão e Espinafre",   Descricao = "Risoto com salmão fresco, espinafre e creme de leite",            PrecoBase = 65.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J18 */ new Prato { Nome = "Peixe ao Coco",                  Descricao = "Filé de peixe branco refogado em leite de coco e pimentão",       PrecoBase = 46.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J19 */ new Prato { Nome = "Hambúrguer de Frango Crispy",    Descricao = "Frango crocante com queijo, alface e molho especial no brioche",   PrecoBase = 28.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J20 */ new Prato { Nome = "Costela Assada no Bafo",         Descricao = "Costela bovina assada lentamente com farofa e vinagrete",          PrecoBase = 74.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J21 */ new Prato { Nome = "Macarrão à Carbonara",           Descricao = "Espaguete com bacon, ovo, queijo parmesão e pimenta-do-reino",    PrecoBase = 39.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J22 */ new Prato { Nome = "Salada Caprese com Salmão",      Descricao = "Tomate, mussarela, salmão defumado e azeite trufado",              PrecoBase = 47.00f, Ativo = true,  Turno = Turno.Jantar },
            /* J23 */ new Prato { Nome = "Porção de Batata Rústica",       Descricao = "Batata rústica assada com ervas finas e queijo parmesão",          PrecoBase = 19.90f, Ativo = true,  Turno = Turno.Jantar },
            /* J24 */ new Prato { Nome = "Contrafilé Acebolado",           Descricao = "Contrafilé grelhado com cebola caramelizada e molho chimichurri",  PrecoBase = 48.00f, Ativo = true,  Turno = Turno.Jantar },
        };
        context.Pratos.AddRange(pratosJantar);

        await context.SaveChangesAsync();

        // ── PratoIngredientes — Almoço ────────────────────────────────────────
        context.PratoIngredientes.AddRange(
            // A00 - Frango Grelhado com Arroz
            new PratoIngrediente { PratoId = pratosAlmoco[0].Id,  IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[0].Id,  IngredienteId = ing[0].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[0].Id,  IngredienteId = ing[4].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[0].Id,  IngredienteId = ing[7].Id  },
            // A01 - Feijão Tropeiro
            new PratoIngrediente { PratoId = pratosAlmoco[1].Id,  IngredienteId = ing[1].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[1].Id,  IngredienteId = ing[13].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[1].Id,  IngredienteId = ing[10].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[1].Id,  IngredienteId = ing[22].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[1].Id,  IngredienteId = ing[27].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[1].Id,  IngredienteId = ing[28].Id },
            // A02 - Macarrão à Bolonhesa
            new PratoIngrediente { PratoId = pratosAlmoco[2].Id,  IngredienteId = ing[11].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[2].Id,  IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[2].Id,  IngredienteId = ing[12].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[2].Id,  IngredienteId = ing[6].Id  },
            // A03 - Frango Xadrez
            new PratoIngrediente { PratoId = pratosAlmoco[3].Id,  IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[3].Id,  IngredienteId = ing[20].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[3].Id,  IngredienteId = ing[26].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[3].Id,  IngredienteId = ing[5].Id  },
            // A04 - Bife Acebolado
            new PratoIngrediente { PratoId = pratosAlmoco[4].Id,  IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[4].Id,  IngredienteId = ing[6].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[4].Id,  IngredienteId = ing[0].Id  },
            // A05 - Arroz com Ovo Frito
            new PratoIngrediente { PratoId = pratosAlmoco[5].Id,  IngredienteId = ing[0].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[5].Id,  IngredienteId = ing[10].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[5].Id,  IngredienteId = ing[8].Id  },
            // A06 - Frango à Parmegiana
            new PratoIngrediente { PratoId = pratosAlmoco[6].Id,  IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[6].Id,  IngredienteId = ing[12].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[6].Id,  IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[6].Id,  IngredienteId = ing[29].Id },
            // A07 - Tilápia Grelhada
            new PratoIngrediente { PratoId = pratosAlmoco[7].Id,  IngredienteId = ing[15].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[7].Id,  IngredienteId = ing[16].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[7].Id,  IngredienteId = ing[0].Id  },
            // A08 - Estrogonofe de Frango
            new PratoIngrediente { PratoId = pratosAlmoco[8].Id,  IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[8].Id,  IngredienteId = ing[17].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[8].Id,  IngredienteId = ing[12].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[8].Id,  IngredienteId = ing[14].Id },
            // A09 - Carne Assada
            new PratoIngrediente { PratoId = pratosAlmoco[9].Id,  IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[9].Id,  IngredienteId = ing[14].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[9].Id,  IngredienteId = ing[35].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[9].Id,  IngredienteId = ing[7].Id  },
            // A10 - Panqueca de Frango
            new PratoIngrediente { PratoId = pratosAlmoco[10].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[10].Id, IngredienteId = ing[12].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[10].Id, IngredienteId = ing[9].Id  },
            // A11 - Macarrão ao Alho e Óleo
            new PratoIngrediente { PratoId = pratosAlmoco[11].Id, IngredienteId = ing[11].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[11].Id, IngredienteId = ing[7].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[11].Id, IngredienteId = ing[25].Id },
            // A12 - PF com Bife
            new PratoIngrediente { PratoId = pratosAlmoco[12].Id, IngredienteId = ing[0].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[12].Id, IngredienteId = ing[1].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[12].Id, IngredienteId = ing[3].Id  },
            // A13 - Frango ao Molho de Cogumelos
            new PratoIngrediente { PratoId = pratosAlmoco[13].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[13].Id, IngredienteId = ing[30].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[13].Id, IngredienteId = ing[17].Id },
            // A14 - Omelete de Queijo e Espinafre
            new PratoIngrediente { PratoId = pratosAlmoco[14].Id, IngredienteId = ing[10].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[14].Id, IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[14].Id, IngredienteId = ing[31].Id },
            // A15 - Steak de Frango com Batata
            new PratoIngrediente { PratoId = pratosAlmoco[15].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[15].Id, IngredienteId = ing[14].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[15].Id, IngredienteId = ing[7].Id  },
            // A16 - Linguiça Acebolada
            new PratoIngrediente { PratoId = pratosAlmoco[16].Id, IngredienteId = ing[28].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[16].Id, IngredienteId = ing[6].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[16].Id, IngredienteId = ing[0].Id  },
            // A17 - Risoto de Queijo e Ervilha
            new PratoIngrediente { PratoId = pratosAlmoco[17].Id, IngredienteId = ing[21].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[17].Id, IngredienteId = ing[29].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[17].Id, IngredienteId = ing[36].Id },
            // A18 - Salada Caesar com Frango
            new PratoIngrediente { PratoId = pratosAlmoco[18].Id, IngredienteId = ing[4].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[18].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[18].Id, IngredienteId = ing[29].Id },
            // A19 - Frango Ensopado com Batata
            new PratoIngrediente { PratoId = pratosAlmoco[19].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[19].Id, IngredienteId = ing[14].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[19].Id, IngredienteId = ing[35].Id },
            // A20 - Macarrão com Frango e Requeijão
            new PratoIngrediente { PratoId = pratosAlmoco[20].Id, IngredienteId = ing[11].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[20].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[20].Id, IngredienteId = ing[17].Id },
            // A21 - Batata Recheada
            new PratoIngrediente { PratoId = pratosAlmoco[21].Id, IngredienteId = ing[14].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[21].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[21].Id, IngredienteId = ing[9].Id  },
            // A22 - Espaguete ao Molho Branco
            new PratoIngrediente { PratoId = pratosAlmoco[22].Id, IngredienteId = ing[11].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[22].Id, IngredienteId = ing[17].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[22].Id, IngredienteId = ing[29].Id },
            // A23 - Caldo de Feijão
            new PratoIngrediente { PratoId = pratosAlmoco[23].Id, IngredienteId = ing[1].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[23].Id, IngredienteId = ing[13].Id },
            new PratoIngrediente { PratoId = pratosAlmoco[23].Id, IngredienteId = ing[7].Id  },
            // A24 - Frango Caipira Refogado
            new PratoIngrediente { PratoId = pratosAlmoco[24].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[24].Id, IngredienteId = ing[6].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[24].Id, IngredienteId = ing[5].Id  },
            new PratoIngrediente { PratoId = pratosAlmoco[24].Id, IngredienteId = ing[7].Id  }
        );

        // ── PratoIngredientes — Jantar ────────────────────────────────────────
        context.PratoIngredientes.AddRange(
            // J00 - Picanha na Brasa
            new PratoIngrediente { PratoId = pratosJantar[0].Id,  IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosJantar[0].Id,  IngredienteId = ing[7].Id  },
            new PratoIngrediente { PratoId = pratosJantar[0].Id,  IngredienteId = ing[8].Id  },
            new PratoIngrediente { PratoId = pratosJantar[0].Id,  IngredienteId = ing[0].Id  },
            // J01 - Costelinha Barbecue
            new PratoIngrediente { PratoId = pratosJantar[1].Id,  IngredienteId = ing[19].Id },
            new PratoIngrediente { PratoId = pratosJantar[1].Id,  IngredienteId = ing[18].Id },
            new PratoIngrediente { PratoId = pratosJantar[1].Id,  IngredienteId = ing[0].Id  },
            // J02 - Hambúrguer Artesanal
            new PratoIngrediente { PratoId = pratosJantar[2].Id,  IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosJantar[2].Id,  IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratosJantar[2].Id,  IngredienteId = ing[4].Id  },
            new PratoIngrediente { PratoId = pratosJantar[2].Id,  IngredienteId = ing[5].Id  },
            new PratoIngrediente { PratoId = pratosJantar[2].Id,  IngredienteId = ing[23].Id },
            // J03 - Salmão Grelhado
            new PratoIngrediente { PratoId = pratosJantar[3].Id,  IngredienteId = ing[32].Id },
            new PratoIngrediente { PratoId = pratosJantar[3].Id,  IngredienteId = ing[33].Id },
            new PratoIngrediente { PratoId = pratosJantar[3].Id,  IngredienteId = ing[16].Id },
            // J04 - Camarão ao Alho e Óleo
            new PratoIngrediente { PratoId = pratosJantar[4].Id,  IngredienteId = ing[24].Id },
            new PratoIngrediente { PratoId = pratosJantar[4].Id,  IngredienteId = ing[7].Id  },
            new PratoIngrediente { PratoId = pratosJantar[4].Id,  IngredienteId = ing[25].Id },
            new PratoIngrediente { PratoId = pratosJantar[4].Id,  IngredienteId = ing[0].Id  },
            // J05 - Frango Assado Inteiro
            new PratoIngrediente { PratoId = pratosJantar[5].Id,  IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosJantar[5].Id,  IngredienteId = ing[7].Id  },
            new PratoIngrediente { PratoId = pratosJantar[5].Id,  IngredienteId = ing[16].Id },
            // J06 - Risoto de Camarão
            new PratoIngrediente { PratoId = pratosJantar[6].Id,  IngredienteId = ing[21].Id },
            new PratoIngrediente { PratoId = pratosJantar[6].Id,  IngredienteId = ing[24].Id },
            new PratoIngrediente { PratoId = pratosJantar[6].Id,  IngredienteId = ing[17].Id },
            // J07 - Filé Mignon ao Molho Madeira
            new PratoIngrediente { PratoId = pratosJantar[7].Id,  IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosJantar[7].Id,  IngredienteId = ing[30].Id },
            new PratoIngrediente { PratoId = pratosJantar[7].Id,  IngredienteId = ing[8].Id  },
            // J08 - Batata Frita com Bacon
            new PratoIngrediente { PratoId = pratosJantar[8].Id,  IngredienteId = ing[14].Id },
            new PratoIngrediente { PratoId = pratosJantar[8].Id,  IngredienteId = ing[13].Id },
            new PratoIngrediente { PratoId = pratosJantar[8].Id,  IngredienteId = ing[9].Id  },
            // J09 - Alcatra ao Molho de Cogumelos
            new PratoIngrediente { PratoId = pratosJantar[9].Id,  IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosJantar[9].Id,  IngredienteId = ing[30].Id },
            new PratoIngrediente { PratoId = pratosJantar[9].Id,  IngredienteId = ing[17].Id },
            // J10 - Penne ao Molho Rosé
            new PratoIngrediente { PratoId = pratosJantar[10].Id, IngredienteId = ing[11].Id },
            new PratoIngrediente { PratoId = pratosJantar[10].Id, IngredienteId = ing[12].Id },
            new PratoIngrediente { PratoId = pratosJantar[10].Id, IngredienteId = ing[17].Id },
            new PratoIngrediente { PratoId = pratosJantar[10].Id, IngredienteId = ing[30].Id },
            // J11 - Carne de Sol
            new PratoIngrediente { PratoId = pratosJantar[11].Id, IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosJantar[11].Id, IngredienteId = ing[0].Id  },
            new PratoIngrediente { PratoId = pratosJantar[11].Id, IngredienteId = ing[8].Id  },
            // J12 - Frango com Mel e Mostarda
            new PratoIngrediente { PratoId = pratosJantar[12].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosJantar[12].Id, IngredienteId = ing[38].Id },
            new PratoIngrediente { PratoId = pratosJantar[12].Id, IngredienteId = ing[39].Id },
            // J13 - Espetinho Misto
            new PratoIngrediente { PratoId = pratosJantar[13].Id, IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosJantar[13].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosJantar[13].Id, IngredienteId = ing[28].Id },
            // J14 - Tilápia ao Molho de Camarão
            new PratoIngrediente { PratoId = pratosJantar[14].Id, IngredienteId = ing[15].Id },
            new PratoIngrediente { PratoId = pratosJantar[14].Id, IngredienteId = ing[24].Id },
            new PratoIngrediente { PratoId = pratosJantar[14].Id, IngredienteId = ing[17].Id },
            // J15 - Macarrão com Frutos do Mar
            new PratoIngrediente { PratoId = pratosJantar[15].Id, IngredienteId = ing[11].Id },
            new PratoIngrediente { PratoId = pratosJantar[15].Id, IngredienteId = ing[24].Id },
            new PratoIngrediente { PratoId = pratosJantar[15].Id, IngredienteId = ing[15].Id },
            new PratoIngrediente { PratoId = pratosJantar[15].Id, IngredienteId = ing[12].Id },
            // J16 - Frango Frito
            new PratoIngrediente { PratoId = pratosJantar[16].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosJantar[16].Id, IngredienteId = ing[7].Id  },
            // J17 - Risoto de Salmão e Espinafre
            new PratoIngrediente { PratoId = pratosJantar[17].Id, IngredienteId = ing[21].Id },
            new PratoIngrediente { PratoId = pratosJantar[17].Id, IngredienteId = ing[32].Id },
            new PratoIngrediente { PratoId = pratosJantar[17].Id, IngredienteId = ing[31].Id },
            new PratoIngrediente { PratoId = pratosJantar[17].Id, IngredienteId = ing[17].Id },
            // J18 - Peixe ao Coco
            new PratoIngrediente { PratoId = pratosJantar[18].Id, IngredienteId = ing[15].Id },
            new PratoIngrediente { PratoId = pratosJantar[18].Id, IngredienteId = ing[26].Id },
            new PratoIngrediente { PratoId = pratosJantar[18].Id, IngredienteId = ing[6].Id  },
            // J19 - Hambúrguer de Frango Crispy
            new PratoIngrediente { PratoId = pratosJantar[19].Id, IngredienteId = ing[2].Id  },
            new PratoIngrediente { PratoId = pratosJantar[19].Id, IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratosJantar[19].Id, IngredienteId = ing[4].Id  },
            new PratoIngrediente { PratoId = pratosJantar[19].Id, IngredienteId = ing[23].Id },
            // J20 - Costela Assada no Bafo
            new PratoIngrediente { PratoId = pratosJantar[20].Id, IngredienteId = ing[19].Id },
            new PratoIngrediente { PratoId = pratosJantar[20].Id, IngredienteId = ing[22].Id },
            new PratoIngrediente { PratoId = pratosJantar[20].Id, IngredienteId = ing[5].Id  },
            // J21 - Carbonara
            new PratoIngrediente { PratoId = pratosJantar[21].Id, IngredienteId = ing[11].Id },
            new PratoIngrediente { PratoId = pratosJantar[21].Id, IngredienteId = ing[13].Id },
            new PratoIngrediente { PratoId = pratosJantar[21].Id, IngredienteId = ing[10].Id },
            new PratoIngrediente { PratoId = pratosJantar[21].Id, IngredienteId = ing[29].Id },
            // J22 - Salada Caprese com Salmão
            new PratoIngrediente { PratoId = pratosJantar[22].Id, IngredienteId = ing[5].Id  },
            new PratoIngrediente { PratoId = pratosJantar[22].Id, IngredienteId = ing[9].Id  },
            new PratoIngrediente { PratoId = pratosJantar[22].Id, IngredienteId = ing[32].Id },
            new PratoIngrediente { PratoId = pratosJantar[22].Id, IngredienteId = ing[25].Id },
            // J23 - Batata Rústica
            new PratoIngrediente { PratoId = pratosJantar[23].Id, IngredienteId = ing[14].Id },
            new PratoIngrediente { PratoId = pratosJantar[23].Id, IngredienteId = ing[29].Id },
            new PratoIngrediente { PratoId = pratosJantar[23].Id, IngredienteId = ing[25].Id },
            // J24 - Contrafilé Acebolado
            new PratoIngrediente { PratoId = pratosJantar[24].Id, IngredienteId = ing[3].Id  },
            new PratoIngrediente { PratoId = pratosJantar[24].Id, IngredienteId = ing[6].Id  },
            new PratoIngrediente { PratoId = pratosJantar[24].Id, IngredienteId = ing[7].Id  }
        );

        await context.SaveChangesAsync();

        // ── Pedidos históricos ────────────────────────────────────────────────
        if (usuarios.Count >= 2)
        {
            var pedido1 = new Pedido
            {
                UsuarioId   = usuarios[0].Id,
                DataHora    = DateTime.Now.AddDays(-3),
                Status      = Status.Entregue,
                TaxaEntrega = 5.00f,
                PrecoTotal  = 0f
            };
            var pedido2 = new Pedido
            {
                UsuarioId   = usuarios[1].Id,
                DataHora    = DateTime.Now.AddHours(-2),
                Status      = Status.Pendente,
                TaxaEntrega = 0f,
                PrecoTotal  = 0f
            };
            context.Pedidos.AddRange(pedido1, pedido2);
            await context.SaveChangesAsync();

            var itens1 = new[]
            {
                new ItemPedido { PedidoId = pedido1.Id, NomePrato = pratosAlmoco[0].Nome, Quantidade = 2, PrecoUnitario = pratosAlmoco[0].PrecoBase, FoiSugestao = false, Observacao = "Sem alface" },
                new ItemPedido { PedidoId = pedido1.Id, NomePrato = pratosJantar[2].Nome, Quantidade = 1, PrecoUnitario = pratosJantar[2].PrecoBase, FoiSugestao = true,  Observacao = ""           },
            };
            var itens2 = new[]
            {
                new ItemPedido { PedidoId = pedido2.Id, NomePrato = pratosJantar[0].Nome, Quantidade = 1, PrecoUnitario = pratosJantar[0].PrecoBase, FoiSugestao = false, Observacao = ""           },
                new ItemPedido { PedidoId = pedido2.Id, NomePrato = pratosAlmoco[2].Nome, Quantidade = 2, PrecoUnitario = pratosAlmoco[2].PrecoBase, FoiSugestao = true,  Observacao = "Bem cozido" },
            };
            context.ItensPedido.AddRange(itens1);
            context.ItensPedido.AddRange(itens2);

            pedido1.PrecoTotal = itens1.Sum(i => i.PrecoUnitario * i.Quantidade) + pedido1.TaxaEntrega;
            pedido2.PrecoTotal = itens2.Sum(i => i.PrecoUnitario * i.Quantidade) + pedido2.TaxaEntrega;
        }

        await context.SaveChangesAsync();

        // Cardápios e demais dados relacionais são gerados por SeedDataCardapio
        await SeedDataCardapio.Popular(serviceProvider, mesas, usuarios);
    }
}
