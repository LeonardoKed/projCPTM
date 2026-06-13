--create database if not exists db_formulario;
CREATE DATABASE IF NOT EXISTS cptm;

-- Use the created database
USE cptm;

-- SQL script to create the 'usuario' table in the database

CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY,
    Nome VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Senha VARCHAR(255) NOT NULL,
    Ativo BOOLEAN DEFAULT TRUE,
    Perfil VARCHAR(50) NOT NULL -- Pode ser 'normal' ou 'admin'
);


CREATE TABLE usuario (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    senha VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    ativo BOOLEAN NOT NULL DEFAULT TRUE
);


INSERT INTO usuario (nome, senha, email, ativo, perfil) VALUES ('leonardo', '123', 'leo.vsanto@hotmail.com', TRUE, 'normal');
INSERT INTO usuario (nome, senha, email, ativo, perfil) VALUES ('Paula', '456', 'paula.miekusz@hotmail.com', TRUE, 'normal');


CREATE TABLE Formulario (
    id INT AUTO_INCREMENT PRIMARY KEY, -- Identificador único para o formulário
    nomeContratada VARCHAR(255) NOT NULL, -- Nome da contratada
    numeroContrato VARCHAR(50) NOT NULL, -- Número do contrato
    localEscopo VARCHAR(255) NOT NULL, -- Local do escopo contratual
    representante VARCHAR(255), -- Representante da contratada
    siglaArea INT, -- Sigla da área
    areaGestora INT, -- Área gestora
    identificadorAreaGestora VARCHAR(255),
    siglaAreaGestora VARCHAR(255),
    supervisoraAmbiental VARCHAR(255),
    autorCadastramento VARCHAR(255),
    responsavelTecnico VARCHAR(255),
    registroProfissional VARCHAR(255),
    documentoResponsabilidadeTecnica VARCHAR(255),
    naturezaPGA INT,
    tipoFormulario VARCHAR(255),
    dataEmissaoFormulario DATE,
    numeroFormulario INT,
    autorFormulario VARCHAR(255),
    arquivoFDC VARCHAR(255),
    codigoFDC INT,
    dataCadastramento DATE,
    horaCadastramento TIME,
    chavePrimaria VARCHAR(255),
    elementoMonitoramentoNumero INT,
    elementoMonitoramentoNome VARCHAR(255),
    municipio INT,
    linhaCPTM INT,
    nomeEstacaoCPTM INT,
    numeroViaLinha INT,
    trechoSentidoLinha INT,
    quilometroPoste VARCHAR(255),
    latitude DECIMAL(10, 7),
    longitude DECIMAL(10, 7),
    atividade INT,
    atividadeNaoListada VARCHAR(255),
    draListado INT,
    draNaoListada VARCHAR(255),
    codigoDRA INT,
    dataValidadeDRA DATE,
    TipoAtividadeCPTM INT,
    NomeEdificacao INT,
    NomeEdificacaoComplemento VARCHAR(255),
    OrigemEfluente INT,
    FonteGeradoraEfluente INT,
    QuantidadeEfluente DOUBLE,
    TipoDestinacaoEfluente INT,
    TipoVeiculo INT,
    IdentificadorVeiculo VARCHAR(255),
    codigoGuiaRemessa VARCHAR(255),
    distanciaViaCPTM DOUBLE,
    observacoesGerais TEXT,
    fotografia1 LONGBLOB,
    fotografia2 LONGBLOB,
    fotografia3 LONGBLOB,
    fotografia4 LONGBLOB,
    usuario_id INT NOT NULL, -- FK para a tabela de usuário
    CONSTRAINT FK_Formulario_Usuario FOREIGN KEY (usuario_id) REFERENCES usuario(id)
);