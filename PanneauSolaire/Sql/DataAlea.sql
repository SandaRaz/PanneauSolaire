TRUNCATE TABLE Secteur cascade;
TRUNCATE TABLE Panneau cascade;
TRUNCATE TABLE Batterie cascade;
TRUNCATE TABLE Salle cascade;
TRUNCATE TABLE InfoSalle;
TRUNCATE TABLE Coupure;
TRUNCATE TABLE SecteurPanneaux;
TRUNCATE TABLE SecteurBatteries;
TRUNCATE TABLE SecteurSalles;
TRUNCATE TABLE Meteo;

------ Secteur
INSERT INTO Secteur(id,refs) VALUES('SEC7001','Secteur 1');

------ Panneau Solaire
INSERT INTO Panneau(id,refs,puissance) VALUES('PAN7001','A',25000);

------ Batterie
INSERT INTO Batterie(id,refs,puissance,limitCons) VALUES('BAT7001','A',19200,50);

------ Salle
INSERT INTO Salle(id,refs,consMoyenne) VALUES('SAL7001','Grande Salle',70);

------ SecteurPanneaux
INSERT INTO SecteurPanneaux(idsecteur,idpanneau) VALUES('SEC7001','PAN7001');

----- SecteurBatteries
INSERT INTO SecteurBatteries(idsecteur,idbatterie) VALUES('SEC7001','BAT7001');

------ SecteurSalle
INSERT INTO SecteurSalles(idsecteur,idsalle) VALUES('SEC7001','SAL7001');