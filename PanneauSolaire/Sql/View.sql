------ VIEW -------
CREATE OR REPLACE VIEW SecteurPanneauxComplet AS
SELECT sp.id,sp.idsecteur,sc.refs as secteur,sp.idpanneau,pn.refs as panneau,pn.puissance
FROM panneau as pn
JOIN secteurPanneaux as sp ON sp.idpanneau = pn.id
JOIN secteur as sc ON sp.idsecteur = sc.id;

CREATE OR REPLACE VIEW SecteurBatteriesComplet AS
SELECT sb.id,sb.idsecteur,sc.refs as secteur,sb.idbatterie,bt.refs as batterie,bt.puissance,bt.limitcons
FROM batterie as bt
JOIN secteurBatteries as sb ON sb.idbatterie = bt.id
JOIN secteur as sc ON sb.idsecteur = sc.id;

CREATE OR REPLACE VIEW SecteurSallesComplet AS
SELECT ss.id,ss.idsecteur,sc.refs as secteur,ss.idsalle,sl.refs as salle,sl.consmoyenne
FROM salle as sl
JOIN secteurSalles as ss ON ss.idsalle = sl.id
JOIN secteur as sc ON ss.idsecteur = sc.id;

CREATE OR REPLACE VIEW SecteurSallesInfosComplet AS
SELECT ss.id,ss.idsecteur,sc.refs as secteur,ss.idsalle,sl.refs as salle,sl.consmoyenne,i.jour,i.heuredebut,i.heurefin,i.nombrepersonne
FROM salle as sl
JOIN infosalle as i ON i.idsalle = sl.id
JOIN secteurSalles as ss ON ss.idsalle = i.idsalle
JOIN secteur as sc ON ss.idsecteur = sc.id;
-------------------

