---
layout: default
title: Aggregation Endpoints
nav_order: 1
---

# Aggregation Endpoints

Aggregation endpoints bring together several native API calls in CreditLens, and then aggregate and enrich the data.

Aggregation Endpoints provide an abstraction layer on top of CreditLens and ensure a reliable service delivery between
versions
as the endpoints are subject to automated tests and quality assurance.

The following Aggregation Endpoints exist:

| Name      | Route                               | Description                                                                                                                     |
|-----------|-------------------------------------|---------------------------------------------------------------------------------------------------------------------------------|
| Spreading | /api/projections/spreading?cif=8114 | An endpoint that presents a clean set of historic and projected statements and calculates macros as specified in the post body. |