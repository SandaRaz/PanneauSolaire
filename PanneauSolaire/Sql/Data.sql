------ Secteur
INSERT INTO Secteur(id,refs) VALUES('SEC7001','Secteur 1');
INSERT INTO Secteur(id,refs) VALUES('SEC7002','Secteur 2');
INSERT INTO Secteur(id,refs) VALUES('SEC7003','Secteur 3');

------ Panneau Solaire
INSERT INTO Panneau(id,refs,puissance) VALUES('PAN7001','A',5000);
INSERT INTO Panneau(id,refs,puissance) VALUES('PAN7002','B',5000);
INSERT INTO Panneau(id,refs,puissance) VALUES('PAN7003','C',5000);
INSERT INTO Panneau(id,refs,puissance) VALUES('PAN7004','D',5000);
INSERT INTO Panneau(id,refs,puissance) VALUES('PAN7005','E',5000);
INSERT INTO Panneau(id,refs,puissance) VALUES('PAN7006','F',5000);

------ Batterie
INSERT INTO Batterie(id,refs,puissance,limitCons) VALUES('BAT7001','A',60000,50);
INSERT INTO Batterie(id,refs,puissance,limitCons) VALUES('BAT7002','B',60000,50);
INSERT INTO Batterie(id,refs,puissance,limitCons) VALUES('BAT7003','C',60000,50);

------ Salle
INSERT INTO Salle(id,refs,consMoyenne) VALUES('SAL7001','Amphi A',50);
INSERT INTO Salle(id,refs,consMoyenne) VALUES('SAL7002','Amphi B',50);
INSERT INTO Salle(id,refs,consMoyenne) VALUES('SAL7003','Amphi C',50);
INSERT INTO Salle(id,refs,consMoyenne) VALUES('SAL7004','Salle 1',50);
INSERT INTO Salle(id,refs,consMoyenne) VALUES('SAL7005','Salle 2',50);
INSERT INTO Salle(id,refs,consMoyenne) VALUES('SAL7006','Salle 5',50);
INSERT INTO Salle(id,refs,consMoyenne) VALUES('SAL7007','Salle 6',50);
INSERT INTO Salle(id,refs,consMoyenne) VALUES('SAL7008','Salle 7',50);

------ SecteurPanneaux
INSERT INTO SecteurPanneaux(idsecteur,idpanneau) VALUES('SEC7001','PAN7001');
INSERT INTO SecteurPanneaux(idsecteur,idpanneau) VALUES('SEC7001','PAN7002');

INSERT INTO SecteurPanneaux(idsecteur,idpanneau) VALUES('SEC7002','PAN7003');

INSERT INTO SecteurPanneaux(idsecteur,idpanneau) VALUES('SEC7003','PAN7004');
INSERT INTO SecteurPanneaux(idsecteur,idpanneau) VALUES('SEC7003','PAN7005');
INSERT INTO SecteurPanneaux(idsecteur,idpanneau) VALUES('SEC7003','PAN7006');

----- SecteurBatteries
INSERT INTO SecteurBatteries(idsecteur,idbatterie) VALUES('SEC7001','BAT7001');
INSERT INTO SecteurBatteries(idsecteur,idbatterie) VALUES('SEC7002','BAT7002');
INSERT INTO SecteurBatteries(idsecteur,idbatterie) VALUES('SEC7003','BAT7003');

------ SecteurSalle
INSERT INTO SecteurSalles(idsecteur,idsalle) VALUES('SEC7001','SAL7001');
INSERT INTO SecteurSalles(idsecteur,idsalle) VALUES('SEC7001','SAL7002');
INSERT INTO SecteurSalles(idsecteur,idsalle) VALUES('SEC7001','SAL7006');

INSERT INTO SecteurSalles(idsecteur,idsalle) VALUES('SEC7002','SAL7004');
INSERT INTO SecteurSalles(idsecteur,idsalle) VALUES('SEC7002','SAL7005');

INSERT INTO SecteurSalles(idsecteur,idsalle) VALUES('SEC7003','SAL7003');
INSERT INTO SecteurSalles(idsecteur,idsalle) VALUES('SEC7003','SAL7007');
INSERT INTO SecteurSalles(idsecteur,idsalle) VALUES('SEC7003','SAL7008');

------ InfoSalle
INSERT INTO InfoSalle(idsalle,jour,heureDebut,heureFin,nombrePersonne)
VALUES('SAL7001','2023-12-05','08:00:00','12:00:00',160);
INSERT INTO InfoSalle(idsalle,jour,heureDebut,heureFin,nombrePersonne)
VALUES('SAL7001','2023-12-05','12:00:00','17:00:00',171);

INSERT INTO InfoSalle(idsalle,jour,heureDebut,heureFin,nombrePersonne)
VALUES('SAL7002','2023-12-05','08:00:00','12:00:00',165);
INSERT INTO InfoSalle(idsalle,jour,heureDebut,heureFin,nombrePersonne)
VALUES('SAL7002','2023-12-05','12:00:00','17:00:00',125);

INSERT INTO InfoSalle(idsalle,jour,heureDebut,heureFin,nombrePersonne)
VALUES('SAL7003','2023-12-05','08:00:00','12:00:00',150);
INSERT INTO InfoSalle(idsalle,jour,heureDebut,heureFin,nombrePersonne)
VALUES('SAL7003','2023-12-05','12:00:00','17:00:00',140);

INSERT INTO InfoSalle(idsalle,jour,heureDebut,heureFin,nombrePersonne)
VALUES('SAL7006','2023-11-28','08:00:00','12:00:00',10);
INSERT INTO InfoSalle(idsalle,jour,heureDebut,heureFin,nombrePersonne)
VALUES('SAL7006','2023-11-28','13:00:00','17:00:00',20);

------ Coupure
INSERT INTO Coupure(id,idsecteur,jour,heurecoupure)
VALUES('COU7001','SEC7001','2023-12-05','10:18:00');