// using Demo.Domain.Model;
// using Demo.Domain.Model.BankAccounts;
// using Demo.Domain.Model.Input;
// using Demo.Domain.Model.Ports;
// using Demo.Domain.Model.User;
//
// namespace Demo.Infrastructure.BankAdapters.Se.Mock
// {
//     public class SeMock01Adapter : BankAdapterBase, ISeMock01Adapter
//     {
//         // Hook Handlers
//         
//         public override async Task<BankPortResult<User>> AuthenticateAsync()
//         {
//             Console.WriteLine("Starting mock SEB authentication...");
//
//             try
//             {
//                 // Simulate state transitions and delays
//                 await SimulateState("Setting up browser", 500);
//                 await SimulateState("Navigating to login page", 500);
//                 await SimulateState("Clicking login button", 500);
//                 await SimulateState("Waiting for QR code to appear", 1000);
//                 await MockRequestQrCodeScan();
//                 await SimulateState("Waiting for user to sign", 1000);
//
//                 // Mock successful user authentication
//                 var mockUser = User.Create("8204161478", "Mock Mocksson");
//                 Console.WriteLine($"Mock authentication successful for user: {mockUser.Nin}, {mockUser.Name}");
//
//                 return BankPortResult<User>.Ok(mockUser);
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Mock authentication failed: {ex.Message}");
//                 return BankPortResult<User>.Fail("Mock authentication error occurred.");
//             }
//         }
//
//         public override Task<BankPortResult<BankAccounts>> FetchBankAccountsAsync()
//         {
//             throw new NotImplementedException();
//         }
//
//         public override Task<BankPortResult<TransactionHistory>> FetchTransactionHistoryAsync()
//         {
//             throw new NotImplementedException();
//         }
//
//         public override Task<BankPortResult<TransactionHistory>> CleanupResourcesAsync()
//         {
//             throw new NotImplementedException();
//         }
//         
//         // Helpers
//
//         private async Task MockRequestQrCodeScan()
//         {
//             Console.WriteLine("Requesting QR code scan...");
//
//             // Simulated QR code data
//             var mockQrCodeData = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAPUAAAD1CAYAAACIsbNlAAAAAXNSR0IArs4c6QAAGLlJREFUeF7tndF62zoMg9v3f+idz3WWdaeSgj8gFbdDr2mKAgGSkpP0/e3t7dfbRf9+/dJCe39/H+5Aff54eOTDff7wO/LhrjVLF/FLbF16kLVmuRzFoGI7y8PIJ+ESidXFkDx/qEFTDvFaZKuKiiSiQhAdZFD3uoLWFU9FDCo2s7WIUCLqMRsi6hsuRBAqcdOpT6QIthG13xEj6oj6zqJ06nEBIsXZl6TvIaKOqCPqTzoiRzkyVfhS1T1E1BF1RP2viLprFHPPo+T5kW3FvkiFJpc5arykm+j1fT56Eh+v3gOJlZz1Ce8qOKbuY7SHaad+dWCzcwwBVyWYCuBvu4iaIUYK26t5R9bvKq4E3Yh68t6YgHjYRtQMsYia4UWsI+qIeskXUqwI8SJqghazjagj6oj6waXYPzd+V1Ryt2pXxKDWwopPPXWsReIitmqsMzuyFrGl66n72HlRVsFbVTvoomxnYLOLsooY1KRXEK9jLRIXsVVjpSIj4iH5JV1VvWAlPslFGdkXwdcev3cGFlGfqXUFEVHPSxXBVi0KnbxNp3bbzuL8XVHc/h8eEV+XrQvZ7rhIV1VFSXymU99QVatNZ8VTyUtIqvoko1U69RpVIsCIGnwPmJI5oh4jRgpIly3N5a5pgxZCdR8Zv+GHLkgido4xpLpXxKWuVzHSdxVM4neU947nZ1NMBe/cPbj7dffwz91+qyJbkYYkTV0vop73VzKZuIKoeJ7wQ50qyHE0ol6gmk59guOStOP5dOozN3mlBT9RFlFH1L9rPilMxDaduuBcr47DGb//0M0lacfz6dQ/vFNXnLk6Kqbrc/Z8xfmbxNZhW5Ezt1iQfZG1iG1HDD/iTF1BkA5wXZ8R9RrBLvGMViVrEVuXIz/2TB1RE2pcw7YiZ13iiaghR0giVNsKgpBtqHG5PtOp06k/I6DyLuM3Ud7NVgWXuK64wCPrvdq2ohB35GGGC1mL2JI8qH4jaoJqRP0EWuNHImoOZYuoeRjaExXvg7WVmFUF8Xaez0i3H8VF8uA+7651rE/eArjYuPtlzNOt7YsyfSlmSRJMEsmi+GodUZ+YqIKoyKO6VkR95iaihiqPqCPqFWVIEYPUk80jahmqNZndaUE9G826ZBeZXL/keWI7SxvJA5kAVJpU7EFdi2CALsrcAEhglNAdsaVTp1P/qE7dIRLqU+1oRHxXtSVV/6q2V8V2dvZU+XWFBkO08+3+QR75tYrvZHtVoRJCR9REen22EfUN21cXgIh6fpu7s1hU5KFPrprniDqivjPFJfRO8ZGRmti6GGiy67WKqCPqiPqTxiLq3oIj/4xOOsT8lrqCpOqFUvLQLAjR/fuvjhd44uLPmLln39maKnHJxRG1HcXmxkUw7nrvS3I2indnXIQfBNudthH1DW1XPBUdMaIeUz+iZiUhoo6oH56p1WIzo146NROlax1RR9QRtaCi73RKjagj6oj6p4n6+GadOl6Rs42A00MTtzq68e68zXVjfQimYUDuG1QudY3qxjbxo7tzpuoBfaHjqpsgBCGZi6hPtCJq/wKP8G5mG1HDX8YgHYZc/Ki2uwsmIVlEHVETvixt1cqUTl0G+dBRRB1RlzEsoi6D0nIUUUfU03OYxawnHiYjLSEusX0i7L8ecQsbWZ98gGbk18X78El8kCOTG6+7VlceRni1XZTtJGPF+E2ESmxJMncSh5BczSURJLmEJBiqse4uIGQPpLhG1AtkiVCJLUlmRJ1OvSo2Ku/SqW9KUgE7zIltRD1GIJ16zox06oVq3HHQBdcV9KyAVPjN+L13KiA5c3mXTp1OfecbIRMpCuRYQQox8evG6661VdRd36d2kzMDgYy+O21J0q5gq34oZhYrye/OPJC4SB7IHohf13YYV0R9wkqSRm5Y3aR1PR9RM2QJP5hnzzqiXrwHJUmLqNl51MWWHAvSqd/e2r56eQVwd5LJq7f7n06nZpgTLjHPnnU6dTr18lKMTCCkaBNBdNl60mHHM3ct8vwQL/J9arIYuWD5ye8s1Y5IRELyQIRK/JJ4SQzEL4mXFIuOm+6Kfal7QK+0CIgR9YlARK3/f+sZXhW8UwUxW4sUppGPiPqGigsESUTXWhF1RF1VrNTClE7dXEAi6og6ov40u3R1z47xiNwLqBW3a+ys8Etys3NiIuNz1x46+HX4VHmTTp1O/ZTGuwRB/JLAVUGQokDWr9iXugckahIY6VwEHHVjpLJ1JZLsq+IDFgSbnTe8bqcmz1dc0JK8qV25Yg9qXBH1AqmtiXg/UvH1jwiV2KprqURanRsJjuodBIlrFhuJi6zXtQc1hog6or4j4JKcTBu7O+pOoe1cazgpkA+fZPxWayW3I4KosE2nZrfyJKMRNUEL3ADmTH0CW1GI1RSRYpNOraLK7abjN3FFznJdI97OzuOuRSo5ESWJi8SgcoGImtiSAjCzJRwlOA7H38H9iMt7ci8QUS8YSwRFkkYERWIgZCQxRNRsVO/ANqK+sZAITa24pBOQDrPzFSAhiCro1ahPuiTJGSl4JAZSHFXekH25vEmnTqe+I+ASj4zUxJaQnBRdtyiQuFxsSSGOqCPqiFoYR4goM34LgH426aqu6hhFOkFFJSf7JWNjB/FI9yW2BEeSH4LtjxB11xnEBdJ9HtaQobkrCLIHEi8hnlvEugqIi417N+E+T4oKLVZqfi/9ibKuwkKEopJfBXx1oeTGRWJQ90ViIoIgtl0xuIWpIi6SBzW/ETXJzM02nXoMGhEqsSUpcv26z6dTP8hWOjWhM3uXSjqEGgURBLFV1z/sXL/u8xF1RE34+tBWHc/oWe7hwjcDIghiq64fUZ9IbX+ltfMc07VWh3hcnx/JBB9P7LiQcn3SLkfWc6e+Li5VFKz/+4ioF+dkSjI1QUR8qs+Ieo1URE2YBL859eqKVzH2uV01omYE68oZ6fSv5u3qaJFO/evXkFEkwRG1/iN4TL5j64j6xEXlXcbvjN9P6a5jnKXHHbcQk+fTqYu+nO+C3pUItWLOSJrxm9WRdGrYqcn/p64Q2RUq/IhSZG9E1KqAKz4LTaRC9qv6dXFR1/lt18Glij0QH3TPij36V7YVROhIBB3bImr2M0cKkciZ77C9KpeIIHcXYjUPEXXzmTqdekzFiFqVKLeLqCNqzprJExVdjgTTMfVV7IH4IPtVbSPqiFrlykM7QuZ06odwPm1w6Vda6tm3gkzEhxoXyQq54b2yLdmziiPZr3u/QooN4UzF+VuNLaK+sYAkSCUjITgh7pVtyZ5VHMl+I+qLf6HDTbr6PL25JX5VkhPiXtlW3e/MTr1YPJ5XOxfJb4fPVaykmaixpVOnU9/1VVEsIuoxAhm/F8wglZx0VFIxiV+V5BWCIth02ar7Tac+ESC8a+nUJACSXDVYCgKJoUOo7vmOEL8CG9JNSFHoygPhoxqviwHNeQf30fhNQCSJ7NgYWZ+Kx/Xt4kiIR2IlflWRkPVpHgiOarwuBhH1DYGImlGfEI94Jn5VkZD1I+o1WmoRS6eG5/cKkqrJ6SQ5OW6on9xy99W5X7UIuYUtnTqd+qkaQYhHFiB+VZGQ9SPqdOoKvix9kGMBCcbtaER8JC7iN6KueU9OOKbyZvrZ751JI8RzbQlxaedwY1Of73r9RUdHNV6XSxU5U2MlOVdFdvjsEO/Mb0R9y2JXglwyjZ6PqE9USM7cPHQVJpJf9R4koo6oH/LdFU+XINy4Hm78k0HXHiJqkoWJbcUoR0apgpC/uEinTqf+TYoRF9Op06kf1h23I3Z1OTeuhxv/rp36OJqQzX13W9LlyF4JwUinV98Rk1grbElcxNaNjeRBPaO6MXU+P8Q2oj4hJ0LrOAfNEr9TEIR8JC5iS2Jw8xBRu2hf5Pl06ppEEKESWze6dOqiH0lwE7Hz+Yi6Bm0iVGLrRhdRR9R3DmX8ZnIiQiW2LIqv1hF1RB1RP6kiIlRi+2Q498ci6oWou8Dp8FsxUnfEdTDN9Usuc9y1Kt7hu6Lsev2lXqp1YUD8ulPj9KuXVyUISToB56r7jahZYXRzTsRHChjxS/Yw5MfsldZVSR5Rz1+/XTVnLvnJvoggyLGAxOAWYrKHiHrBrp1JIyR3CULWIt2E+CW2pGgTbDJ+F5wFuwhCkk4qXkTdNwFE1AxbwtthYZv9f2oiHlIx3ZHH3TC5VHOFPiPzT8C2CxtSAFzeuc+TxuXylly6tn2ho2LDpACoZIioT6RcbCPqvd03on7i7Ox2T7WoHHbuWlcomBF1RH3nPOkQxFYVVTp1OvWKK1comLP41EKa8fuGoNs91aKSTk2Q4rZElDlTwxtxAm6FrZr+dOp06nTqTwio7X/Wjchhn9wck3GF3EJW7HcUG/HrdhO12K1ypvogBVP1WWXnHuW6cubub7ivK7zSIhtzRek+31VsKjB4NfEi6nkWCe8IFyJq+AsnRCQkacRvOjWh+Nw2nXrzaxeSNlc87vPp1Ew4FWM94ccVjmKkELt7S6dOp15yiBQ8clfg+nWJP7vLIXF1TVfu3mxRuwFc+XmSNPf1V8fzu7El46xrW7E3IuCK9VQfLu+GUwG5KFMD/Y52Lrivfn435q5QyWvMir1F1HBMrQD91T5eLUp3/d34RdQ1iHfkHX2irGYb1/Tigvvq53ejGlHXIO7yJuP3Ig8uuK9+voZiupeIWsdqZenyJqKOqGuYCL+66RaAiqBzpoZn6lcnjVy6dCWXfJqK4FVBaNcH6SbDzvF+/L7l838E29kqHXsgcRFbsocRn0vO1ISkLriENCSu5yl3PkmStjMud1+rvam+3UJKsCWCUOM/7MhrSNeW7CGihhMISTohXkRNkGUFkwiCROEKlfCD7CGijqgJj++27nSVTt1XmCLqiDqifgqBHzJ+k7MrwYmMnsSWxODakriI7Sgu0iXJiEgw6Oq0JAbXtmMPxKebRzSSzz4mGlHPaUSESmwjale68+eJANU8EJ8R9Q1VVxBdFCFxEVuVTLN9pVNH1B839enUXPpEqMQ2oua5UJ8gXVXNA/GZTp1O/dTNczp1OvVHp57910u1ApIDvOuTjJ1dax1+SdVV4+iq+hWYXXXacDHbWQQJZ8gnJId3XxG1Krs/diRBqneXoOo6v+3c9dznSbHZ+cGNity6xSKibnz3vHMK6RIJEQ/Zb1e8RBBuDGQtUjRdvxF1RE34drftEAQpCqTYpFOfaKlTRM7UT0hCBZe4dkVG1vq4TAHfnMqZmqGbTs3wGloTghYsJ1dMshbZQ0VRcddzn0+nnrPDHr8rfniQkIyQYbRtstbwZnDSoa7ajcgeCF4783CFtVTekFgrjgUdOWv7PjWpxKSjqcmh60fUJAv6+Y6O+oTkpOCpvImob6iqgO1OsJv0igSrMXRhuLubqPslJaQCG9VHRc6JD1LEVL/p1IvCpIJIbiZnhU0lHS2MEfWZYBXfipwTHxE1SE7G7zWZu4iXTs3eLLSIevaJMrWy0c7jXve7pOnqXLMisvOs7mLjjsTurW1nISZ8Jjh02JKCO+RXRH2mhQBJEhlR66NvRM25GFHDX/0k4k2nnhdGt0uSCYDYVuS3wwdpMBF1RF3GQTKBRNQM9ogajs45UzOCVUwgETXDPKKOqJeMIQQh1EunJmgxW5KzYR6u/HNGDIqv1qRDkFt5YqvugZwFiS29fFJfsXRgQC8su/Kr5ozYuUJFa0XU8xtHMqoTgo0SRIRKbCNqnl8iINU2olaRemBHhEY6D7FVt0KESmwj6oi6/XxGzmKqIMhlDiF5OvWJlpozUkRJHnbn1+UdmcTU4w6J6dI/EUw24oJDui+xVfdAui+x7RJPBwY5U8+LqMqjDwzJmbqLIMQv6QaEeOTM4xYQUsnd/ZIuRzBQuzch42wq2MmPLrwq/KqYR9Q3tAmhI2p9JI+o1wioQiXHoIg6or6zjhQ2QkYibHcycZ+v6KjufsnxamQbUUfUEbWgwp1FLKJeJCRn6jk4Lknd50lHzJl6foGWTt34hQ4y9uWijBWbiLpI1F3nK7fCE/GQPbiXX2QtgoG7X/KunXRPMgWRIiZMwncTgg3xu9OW4Kjatv1GmXsuIAQjlZwkjJAmomY/hkDwquACyftOW1WoR0yqbUS9yGBEzca+Lrwi6hOBiBp+JTPjNzvnqgTrmqIOv6SI7Oy+ZC2Co2qbTp1O/ZCD6h0AEVnGb9Z9Uaeu+P/UatKvMEZ1nfUfKuOBwc4LrVmXI0Jzc07wItiQwjKKgWBA+LyTdyX/9dJNsJsIQpCd4JK4CHGvQCY3513YuFyKqG+ZcRPsJoIQJKJml18VBYTkh9xtqGdMsn5EHVETvixt06nZRR05Y5IkRdQRNeFLRP0kWqTguVNfRB1RP0nTr48R4laMxB1HpgpBZPz2/1tMybe0yC0iIROxVclQcabe2Q261urw6/qcFauKnKn8IDFU7JcUQnW9iPqJaUMFlxCEdF8yHhBBuH5dXCheHUWfxFCx34gafkqMJN21dQURUc8RJIWpQyRdl3Izvy4X0qnTqUk9utt2vE4iXfKwJYU44/dTaf77oYrq2pG0rrgIZF3dxL3bcPdQMY66e+jCtquIdcRb8tlvQgaS+A4gK0AkPlRsXFzUdTrtum7wu2ImeSQNpiuXqt+IesGYCpKqhFQTRs9h6voVdhV4ERzcmCNqF8HF2ZWcpdyku4nsEhXZF9lDUdokNxH1CVNXLlW/6dTp1JJgFaOIOqJ+yJOcqccQpVM/pI5kQHD8Vmfq2fep1VZPx1ECjpQZaFTRTciS7n7d50msM1s3BsKlUQyu+AgGZC3it8t2mJuI+oS7K5kdguiKNaLu40FE3YVA4+9+7xRERD0nyM6poJGmsut06ohaJstnw45pgwRCilhE/fY2/TkjAo4LOnmekGFkmzM1RzCi5pjteiKdOp36Ka5F1E/BtuWhYW4q/un8luhvi5DXXDttd2JAzu8kLndi6pqCrjA1khh23uCPctb2LS1CJmK7U6hdJCX7JbY7ibfzaEP2RQoTmUBIDBE1YS341yOz11REqMQWbqPFfCfxImqWQrfYzFZLp4Zn6oiaEbcLL1KsXPGQr+YSdNy4IuoXnL9JgrtsCfndETGdmmVxq6gr/u0O255u7ZK0g7iHTzcu916AXJSRzkNsCbYdeHXl4arY6qpZvKcmTrpsXTIQ4pE9uHFF1ATt+Uc3O/IQUbPcYGs3aRE1E0Q69Zyi7k25iy0RT8k/yCMLEtuImn3B4NXE6yLubr/qfQGJi9gSjQwbV87UHEK32GT8Zph3CcK9vCJxEVuGzlfrdOonEIyox6B1EXe33x/bqV3iEq2QpJEuVxFDR4LJWd8dqa+AAYnBtSW8dTs1iZWsRfwO+VHxIwkkiA5Ck0TOYiWgdwjNLWzHvlwcujBw+UGeJxi4+yVxkbWI34h6gRYBPaL2/zsGIS6xjaiLvk9NQE+n9s+jpKuT3HQVNhKDaxtRR9R3DnURWiUZESqxJSLpwoDE4NqqeB/ruPslsZK1iF97/K4IzB1d3RjIlw6IrZsI8ryLAVmr4qxO1nMvQndjQ/Y2snWLUER9kW9pkUSS44pLsNnzbrwkroh6jpaKDfqNsooqmE6995aaCCqirkCL+SAFM6KeYEtGamJLUkkSmU7NimBF4yG5dG0JFyLqiNrl2/15Qjx3UZW4s3Uianj7XQFYxm/WedKpGV4VHHULE3meFEy14P3oMzUBrEs8O2MgBZMQT721JYKqONq8GluCIcGG+P3nbr93Jr3rkomQIaImcsh76g+0CMEIycmHKUgMEXVNztKp2RGga+qT80C+0EEEFVGfCOwsLOnU6dQfzTeinhOhq4gR6pEYImqCLJtidhZnsoucqRdoqTeLs2MIufghBOmKi4yIbrEgzxNsyB5UoVTkkeRMjYvwLp36hipJRJetStIK4qlrzY4QXRNERM0mx3TqdOplY9h5YUnWcrsZeb6iYJKiT2JT/aZTp1PfeUWElk49vwRVxUcEnfH7hhYZ5UgiumzVkbiim6hrZfw+kdqZ85nY1RjSqdOp06k/qaiiYKriu0SnpkGo9mTsczuMGtOsOs86F/Hr7oHgRWztDvF+9Ajtz52itFX+WKnrVeC1M7/Dtch7agqkau8C6T5PyBxRn2iR11QjfFWRrYqryi+SsytwyY0Bjd8ERGJrb2LSIQhx3OpK9uuuRfAitqS4RdQs4yQPxDadmuVh+ll3t1hE1Ozjs+SmfZZiNWeuoEhhpBOPvIeM33OldyU4oo6of3OATDy2qGFTazFXN1xxY0k24N5ukueJrbsHt8uRLnWFnBG8iK0qvtl9gYvNt/sHeYTkxJYkzfVLnie27h4iaoLg3DaiXuCYTs0+9EAoSc6phKTq0cLtRmSvs45Ifaj2BC9StFXbdGo1U5/sVHB/yjhKSBpR+/cFbsGLqCPqhwhE1A8h+suA4EUahGr7H17fmzrknKBDAAAAAElFTkSuQmCC";
//             Console.WriteLine($"Mock QR code generated: {mockQrCodeData}");
//
//             // Request input using RequestInputAsync
//             await RequestInputAsync(InputRequestType.Challenge, new Dictionary<string, string?>
//             {
//                 { "qrCodeData", mockQrCodeData }
//             });
//
//             Console.WriteLine("Simulated user scanning QR code...");
//             await Task.Delay(1000); // Simulate user scanning time
//         }
//
//         private async Task SimulateState(string stateDescription, int delayMs)
//         {
//             Console.WriteLine(stateDescription);
//             await Task.Delay(delayMs);
//         }
//     }
// }
