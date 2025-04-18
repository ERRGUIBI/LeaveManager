# LeaveManagerAPI

## Description
LeaveManagerAPI est une API .NET Core permettant de gérer les demandes de congés des employés. Elle prend en charge plusieurs types de congés (annuels, maladie, etc.), la validation des demandes et le suivi des congés restants.

## Fonctionnalités
- CRUD pour les demandes de congés.
- Validation des dates de congé pour éviter les chevauchements.
- Filtrage, pagination et tri des demandes.
- Rapport des congés par année et par employé.
- Système d'approbation des demandes par un administrateur.

## Prérequis
- **.NET 6.0** ou version supérieure
- **Docker** (si tu veux déployer avec Docker)

## Installation

1. Clone le dépôt:

```bash
git clone https://github.com/ton-utilisateur/LeaveManagerAPI.git
cd LeaveManagerAPI
