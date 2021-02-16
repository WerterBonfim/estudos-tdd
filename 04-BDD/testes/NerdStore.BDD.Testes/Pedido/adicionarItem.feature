Funcionalidade: Pedido - Adicionar Item ao Carrinho
    como um usuário
    eu desejo colocar um item no carrinho
    Para que eu posso comprá-lo posteriormente


Cenário: Adicionar item com sucesso a um novo pedido
Dado Que um produto esteja na vitrine
E Esteja disponivel no estoque
E O usuario esteja logado
Quando O usuário adiconar uma unidade ao carrinho
Então O usuário será redirecionado ao resumo da compra
E O valor total do poedido será exatamente o valor do item adicionado