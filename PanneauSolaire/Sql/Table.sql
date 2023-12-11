CREATE DATABASE pansol;
\c pansol;

------ TABLE ------
CREATE TABLE Secteur(
	id varchar(7) PRIMARY KEY NOT NULL,
	refs varchar(20)
);

CREATE TABLE Panneau(
	id varchar(7) PRIMARY KEY NOT NULL,
	refs varchar(20),
	puissance double precision
);

CREATE TABLE Batterie(
	id varchar(7) PRIMARY KEY NOT NULL,
	refs varchar(20),
	puissance double precision,
	limitCons double precision
);

CREATE TABLE Salle(
	id varchar(7) PRIMARY KEY NOT NULL,
	refs varchar(20),
	consMoyenne double precision
);

CREATE TABLE Meteo(
	id SERIAL PRIMARY KEY NOT NULL,
	jour Date,
	heure Time,
	lumiere double precision,
	lumiereMax double precision
);

CREATE TABLE Coupure(
	id varchar(7) PRIMARY KEY NOT NULL,
	idSecteur varchar(7),
	jour Date,
	heureCoupure Time
);
CREATE SEQUENCE sequence_coupure START WITH 1;	
ALTER TABLE Coupure ADD FOREIGN KEY(idSecteur) REFERENCES Secteur(id);

CREATE TABLE InfoSalle(
	id SERIAL PRIMARY KEY NOT NULL,
	idSalle varchar(7),
	jour Date,
	heureDebut Time,
	heureFin Time,
	nombrePersonne double precision
);
ALTER TABLE InfoSalle ADD FOREIGN KEY(idSalle) REFERENCES Salle(id);

--------------- x -----------------

CREATE TABLE SecteurPanneaux(
	id SERIAL PRIMARY KEY NOT NULL,
	idSecteur varchar(7),
	idPanneau varchar(7)
);
ALTER TABLE SecteurPanneaux ADD FOREIGN KEY(idSecteur) REFERENCES Secteur(id);
ALTER TABLE SecteurPanneaux ADD FOREIGN KEY(idPanneau) REFERENCES Panneau(id);

CREATE TABLE SecteurBatteries(
	id SERIAL PRIMARY KEY NOT NULL,
	idSecteur varchar(7),
	idBatterie varchar(7)
);
ALTER TABLE SecteurBatteries ADD FOREIGN KEY(idSecteur) REFERENCES Secteur(id);
ALTER TABLE SecteurBatteries ADD FOREIGN KEY(idBatterie) REFERENCES Batterie(id);

CREATE TABLE SecteurSalles(
	id SERIAL PRIMARY KEY NOT NULL,
	idSecteur varchar(7),
	idSalle varchar(7)
);
ALTER TABLE SecteurSalles ADD FOREIGN KEY(idSecteur) REFERENCES Secteur(id);
ALTER TABLE SecteurSalles ADD FOREIGN KEY(idSalle) REFERENCES Salle(id);

-------------------
---------- REQUEST AND JOIN -----------
SELECT min(heureDebut),max(heureFin) FROM infosalle WHERE jour='2023-12-05';
SELECT nombrePersonne FROM infosalle where '06:00:00'>heureDebut AND '06:00:00'<heureFin and jour='2023-12-05';
SELECT nombrePersonne FROM InfoSalle WHERE idSalle='SAL7001' AND jour='2023-12-05' AND heureDebut<'08:00' AND heureFin>'08:00';

SELECT * FROM InfoSalle WHERE EXTRACT(ISODOW FROM InfoSalle.jour) = 6;
SELECT * FROM InfoSalle WHERE idSalle='SAL7001' AND (EXTRACT(ISODOW FROM InfoSalle.jour) = 7) AND heureDebut<='08:00' AND heureFin>'08:00';

SELECT * FROM Meteo WHERE ABS(EXTRACT(EPOCH FROM (heure::time - '09:15'::time)) / 60) <= 30;