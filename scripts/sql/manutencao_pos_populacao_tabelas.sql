update usuario_adm set ids_cnae = null where ids_cnae = "";
update usuario_adm set ids_localidade = null where ids_localidade = "";
update usuario_adm set ids_gruc = null where ids_gruc = "";


update modulos set criar = 'N'
where criar is null;

update modulos set consultar = 'N'
where consultar is null;

update modulos set comentar = 'N'
where comentar is null;

update modulos set alterar = 'N'
where alterar is null;

update modulos set excluir = 'N'
where excluir is null;

update modulos set aprovar = 'N'
where aprovar is null;


update doc_sind
set abrangencia = null where abrangencia = '[]';

update doc_sind 
set cnae_doc = null where cnae_doc = '[]';

update doc_sind 
set sind_patronal = null where sind_patronal  = '[]';

update doc_sind 
set sind_laboral = null where sind_laboral = '[]';
