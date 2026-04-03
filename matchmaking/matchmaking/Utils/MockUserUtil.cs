using Microsoft.WindowsAppSDK.Runtime.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace matchmaking.Utils
{
    internal class MockUserUtil
    {
        private List<UserData> users = new List<UserData>
        {
            new UserData(0, "florina_d", "florina.d@example.com", "+40721000006", "https://example.com/avatars/6.jpg", "Yoga and meditation fan.", new DateTime(1999, 4, 14)),
            new UserData(1, "alex_m", "alex.m@example.com", "+40721000001", "https://example.com/avatars/1.jpg", "Love hiking and coffee.", new DateTime(1995, 3, 12)),
            new UserData(2, "bianca_p", "bianca.p@example.com", "+40721000002", "https://example.com/avatars/2.jpg", "Bookworm and cat lover.", new DateTime(1998, 7, 24)),
            new UserData(3, "chris_t", "chris.t@example.com", "+40721000003", "https://example.com/avatars/3.jpg", "Gym enthusiast and cook.", new DateTime(1993, 11, 5)),
            new UserData(4, "diana_r", "diana.r@example.com", "+40721000004", "https://example.com/avatars/4.jpg", "Traveler and photographer.", new DateTime(1997, 1, 18)),
            new UserData(5, "evan_s", "evan.s@example.com", "+40721000005", "https://example.com/avatars/5.jpg", "Music producer by night.", new DateTime(1994, 9, 30)),
            new UserData(6, "florina_d", "florina.d@example.com", "+40721000006", "https://example.com/avatars/6.jpg", "Yoga and meditation fan.", new DateTime(1999, 4, 14)),
            new UserData(7, "george_b", "george.b@example.com", "+40721000007", "https://example.com/avatars/7.jpg", "Software dev and gamer.", new DateTime(1996, 6, 22)),
            new UserData(8, "hana_v", "hana.v@example.com", "+40721000008", "https://example.com/avatars/8.jpg", "Artist and dog mom.", new DateTime(2000, 2, 9)),
            new UserData(9, "ion_c", "ion.c@example.com", "+40721000009", "https://example.com/avatars/9.jpg", "Football fanatic.", new DateTime(1992, 8, 17)),
            new UserData(10, "julia_n", "julia.n@example.com", "+40721000010", "https://example.com/avatars/10.jpg", "Chef and food blogger.", new DateTime(1997, 12, 3)),
            new UserData(11, "kevin_o", "kevin.o@example.com", "+40721000011", "https://example.com/avatars/11.jpg", "Runner and nature lover.", new DateTime(1995, 5, 28)),
            new UserData(12, "laura_f", "laura.f@example.com", "+40721000012", "https://example.com/avatars/12.jpg", "Interior design obsessed.", new DateTime(1998, 10, 11)),
            new UserData(13, "mihai_g", "mihai.g@example.com", "+40721000013", "https://example.com/avatars/13.jpg", "Cyclist and coffee addict.", new DateTime(1993, 7, 7)),
            new UserData(14, "nadine_h", "nadine.h@example.com", "+40721000014", "https://example.com/avatars/14.jpg", "Fashion and travel junkie.", new DateTime(2001, 3, 19)),
            new UserData(15, "oliver_k", "oliver.k@example.com", "+40721000015", "https://example.com/avatars/15.jpg", "Board games every weekend.", new DateTime(1996, 1, 25)),
            new UserData(16, "paula_m", "paula.m@example.com", "+40721000016", "https://example.com/avatars/16.jpg", "Nurse with a big heart.", new DateTime(1994, 11, 8)),
            new UserData(17, "radu_e", "radu.e@example.com", "+40721000017", "https://example.com/avatars/17.jpg", "Loves jazz and old movies.", new DateTime(1990, 6, 14)),
            new UserData(18, "sofia_l", "sofia.l@example.com", "+40721000018", "https://example.com/avatars/18.jpg", "Plant parent and reader.", new DateTime(1999, 9, 2)),
            new UserData(19, "tudor_a", "tudor.a@example.com", "+40721000019", "https://example.com/avatars/19.jpg", "Entrepreneur and skier.", new DateTime(1991, 4, 20)),
            new UserData(20, "ursula_b", "ursula.b@example.com", "+40721000020", "https://example.com/avatars/20.jpg", "Dancer and theater fan.", new DateTime(2000, 8, 6)),
            new UserData(21, "vlad_c", "vlad.c@example.com", "+40721000021", "https://example.com/avatars/21.jpg", "Into astronomy and sci-fi.", new DateTime(1995, 2, 13)),
            new UserData(22, "wendy_d", "wendy.d@example.com", "+40721000022", "https://example.com/avatars/22.jpg", "Loves baking and DIY.", new DateTime(1997, 7, 31)),
            new UserData(23, "xander_f", "xander.f@example.com", "+40721000023", "https://example.com/avatars/23.jpg", "Surfer and beach bum.", new DateTime(1993, 12, 16)),
            new UserData(24, "yara_g", "yara.g@example.com", "+40721000024", "https://example.com/avatars/24.jpg", "Minimalist and meditator.", new DateTime(1998, 5, 4)),
            new UserData(25, "zack_h", "zack.h@example.com", "+40721000025", "https://example.com/avatars/25.jpg", "Podcaster and night owl.", new DateTime(1996, 10, 27)),
            new UserData(26, "ana_i", "ana.i@example.com", "+40721000026", "https://example.com/avatars/26.jpg", "Loves wine and sunsets.", new DateTime(1994, 3, 9)),
            new UserData(27, "bogdan_j", "bogdan.j@example.com", "+40721000027", "https://example.com/avatars/27.jpg", "Rock climber and camper.", new DateTime(1992, 8, 23)),
            new UserData(28, "carmen_k", "carmen.k@example.com", "+40721000028", "https://example.com/avatars/28.jpg", "Teacher and puzzle solver.", new DateTime(1999, 1, 15)),
            new UserData(29, "dan_l", "dan.l@example.com", "+40721000029", "https://example.com/avatars/29.jpg", "Car enthusiast and chef.", new DateTime(1991, 6, 7)),
            new UserData(30, "elena_m", "elena.m@example.com", "+40721000030", "https://example.com/avatars/30.jpg", "Pilates and green juice.", new DateTime(2000, 11, 29)),
            new UserData(31, "felix_n", "felix.n@example.com", "+40721000031", "https://example.com/avatars/31.jpg", "Architect and city walker.", new DateTime(1995, 4, 18)),
            new UserData(32, "gabi_o", "gabi.o@example.com", "+40721000032", "https://example.com/avatars/32.jpg", "Loves karaoke and brunch.", new DateTime(1997, 9, 10)),
            new UserData(33, "horia_p", "horia.p@example.com", "+40721000033", "https://example.com/avatars/33.jpg", "Marathon runner.", new DateTime(1990, 2, 26)),
            new UserData(34, "ioana_r", "ioana.r@example.com", "+40721000034", "https://example.com/avatars/34.jpg", "Marine biology student.", new DateTime(2002, 7, 3)),
            new UserData(35, "jasper_s", "jasper.s@example.com", "+40721000035", "https://example.com/avatars/35.jpg", "Vintage collector.", new DateTime(1993, 12, 21)),
            new UserData(36, "kira_t", "kira.t@example.com", "+40721000036", "https://example.com/avatars/36.jpg", "Sci-fi writer and coder.", new DateTime(1998, 5, 14)),
            new UserData(37, "liviu_u", "liviu.u@example.com", "+40721000037", "https://example.com/avatars/37.jpg", "Bass player in a band.", new DateTime(1994, 10, 8)),
            new UserData(38, "mara_v", "mara.v@example.com", "+40721000038", "https://example.com/avatars/38.jpg", "Loves festivals and art.", new DateTime(1999, 3, 25)),
            new UserData(39, "nick_w", "nick.w@example.com", "+40721000039", "https://example.com/avatars/39.jpg", "Startup founder.", new DateTime(1991, 8, 12)),
            new UserData(40, "oana_x", "oana.x@example.com", "+40721000040", "https://example.com/avatars/40.jpg", "Nurse and weekend hiker.", new DateTime(1996, 1, 6)),
            new UserData(41, "petru_y", "petru.y@example.com", "+40721000041", "https://example.com/avatars/41.jpg", "History buff.", new DateTime(1992, 6, 19)),
            new UserData(42, "raluca_z", "raluca.z@example.com", "+40721000042", "https://example.com/avatars/42.jpg", "Fashion designer.", new DateTime(2000, 11, 11)),
            new UserData(43, "stefan_a", "stefan.a@example.com", "+40721000043", "https://example.com/avatars/43.jpg", "Loves chess and coding.", new DateTime(1995, 4, 30)),
            new UserData(44, "teodora_b", "teodora.b@example.com", "+40721000044", "https://example.com/avatars/44.jpg", "Vet and animal rescuer.", new DateTime(1997, 9, 22)),
            new UserData(45, "ugo_c", "ugo.c@example.com", "+40721000045", "https://example.com/avatars/45.jpg", "Loves travel and pasta.", new DateTime(1993, 2, 15)),
            new UserData(46, "viorica_d", "viorica.d@example.com", "+40721000046", "https://example.com/avatars/46.jpg", "Singer and music teacher.", new DateTime(1998, 7, 8)),
            new UserData(47, "walter_e", "walter.e@example.com", "+40721000047", "https://example.com/avatars/47.jpg", "Woodworker and camper.", new DateTime(1990, 12, 1)),
            new UserData(48, "xenia_f", "xenia.f@example.com", "+40721000048", "https://example.com/avatars/48.jpg", "Loves yoga and smoothies.", new DateTime(2001, 5, 17)),
            new UserData(49, "yannis_g", "yannis.g@example.com", "+40721000049", "https://example.com/avatars/49.jpg", "Diver and ocean activist.", new DateTime(1994, 10, 9)),
            new UserData(50, "zara_h", "zara.h@example.com", "+40721000050", "https://example.com/avatars/50.jpg", "Illustrator and tea lover.", new DateTime(1999, 3, 4)),
            new UserData(51, "andrei_i", "andrei.i@example.com", "+40721000051", "https://example.com/avatars/51.jpg", "Cyclist and foodie.", new DateTime(1996, 8, 26)),
            new UserData(52, "beatrice_j", "beatrice.j@example.com", "+40721000052", "https://example.com/avatars/52.jpg", "Journalist and bookworm.", new DateTime(1993, 1, 13)),
            new UserData(53, "cosmin_k", "cosmin.k@example.com", "+40721000053", "https://example.com/avatars/53.jpg", "DJ and night life lover.", new DateTime(1991, 6, 5)),
            new UserData(54, "daria_l", "daria.l@example.com", "+40721000054", "https://example.com/avatars/54.jpg", "Psychologist and runner.", new DateTime(1997, 11, 28)),
            new UserData(55, "emilian_m", "emilian.m@example.com", "+40721000055", "https://example.com/avatars/55.jpg", "Engineer and biker.", new DateTime(1992, 4, 20)),
            new UserData(56, "francesca_n", "francesca.n@example.com", "+40721000056", "https://example.com/avatars/56.jpg", "Loves opera and wine.", new DateTime(2000, 9, 14)),
            new UserData(57, "gabriel_o", "gabriel.o@example.com", "+40721000057", "https://example.com/avatars/57.jpg", "Architect and chess player.", new DateTime(1995, 2, 7)),
            new UserData(58, "henrieta_p", "henrieta.p@example.com", "+40721000058", "https://example.com/avatars/58.jpg", "Loves pottery and hiking.", new DateTime(1998, 7, 1)),
            new UserData(59, "iulian_r", "iulian.r@example.com", "+40721000059", "https://example.com/avatars/59.jpg", "Gamer and anime fan.", new DateTime(2001, 12, 23)),

            new UserData(60, "janine_s", "janine.s@example.com", "+40721000060", "https://example.com/avatars/60.jpg","i like computers", new DateTime(2010, 5, 16)),

            new UserData(61, "kristian_t", "kristian.t@example.com", "+40721000061", "https://example.com/avatars/61.jpg", "Loves skiing and jazz.", new DateTime(1990, 10, 8)),
            new UserData(62, "larisa_u", "larisa.u@example.com", "+40721000062", "https://example.com/avatars/62.jpg", "Dancer and event planner.", new DateTime(1996, 3, 31)),
            new UserData(63, "marius_v", "marius.v@example.com", "+40721000063", "https://example.com/avatars/63.jpg", "Football coach.", new DateTime(1993, 8, 24)),
            new UserData(64, "nicoleta_w", "nicoleta.w@example.com", "+40721000064", "https://example.com/avatars/64.jpg", "Loves painting and cats.", new DateTime(1999, 1, 17)),
            new UserData(65, "octavian_x", "octavian.x@example.com", "+40721000065", "https://example.com/avatars/65.jpg", "Lawyer and wine collector.", new DateTime(1988, 6, 10)),
            new UserData(66, "petra_y", "petra.y@example.com", "+40721000066", "https://example.com/avatars/66.jpg", "Loves road trips.", new DateTime(2000, 11, 2)),
            new UserData(67, "quentin_z", "quentin.z@example.com", "+40721000067", "https://example.com/avatars/67.jpg", "Filmmaker and cinephile.", new DateTime(1995, 4, 25)),
            new UserData(68, "roxana_a", "roxana.a@example.com", "+40721000068", "https://example.com/avatars/68.jpg", "Loves salsa dancing.", new DateTime(1997, 9, 18)),
            new UserData(69, "silviu_b", "silviu.b@example.com", "+40721000069", "https://example.com/avatars/69.jpg", "Entrepreneur and surfer.", new DateTime(1992, 2, 11)),
            new UserData(70, "tatiana_c", "tatiana.c@example.com", "+40721000070", "https://example.com/avatars/70.jpg", "Loves baking and Netflix.", new DateTime(2001, 7, 4)),
            new UserData(71, "ungur_d", "ungur.d@example.com", "+40721000071", "https://example.com/avatars/71.jpg", "Loves mountains and beer.", new DateTime(1994, 12, 27)),
            new UserData(72, "valentina_e", "valentina.e@example.com", "+40721000072", "https://example.com/avatars/72.jpg", "Nutritionist and runner.", new DateTime(1998, 5, 20)),
            new UserData(73, "william_f", "william.f@example.com", "+40721000073", "https://example.com/avatars/73.jpg", "Loves photography.", new DateTime(1991, 10, 13)),
            new UserData(74, "xandra_g", "xandra.g@example.com", "+40721000074", "https://example.com/avatars/74.jpg", "Art curator and traveler.", new DateTime(1996, 3, 6)),
            new UserData(75, "yulia_h", "yulia.h@example.com", "+40721000075", "https://example.com/avatars/75.jpg", "Loves ice skating.", new DateTime(2000, 8, 29)),
            new UserData(76, "zeno_i", "zeno.i@example.com", "+40721000076", "https://example.com/avatars/76.jpg", "Philosopher and reader.", new DateTime(1989, 1, 21)),
            new UserData(77, "alina_j", "alina.j@example.com", "+40721000077", "https://example.com/avatars/77.jpg", "Loves concerts and coffee.", new DateTime(1995, 6, 14)),
            new UserData(78, "bogdan_k", "bogdan.k@example.com", "+40721000078", "https://example.com/avatars/78.jpg", "Swimmer and gym rat.", new DateTime(1993, 11, 7)),
            new UserData(79, "corina_l", "corina.l@example.com", "+40721000079", "https://example.com/avatars/79.jpg", "Loves cooking and poetry.", new DateTime(1999, 4, 1)),
            new UserData(80, "dragos_m", "dragos.m@example.com", "+40721000080", "https://example.com/avatars/80.jpg", "Car mechanic and gamer.", new DateTime(1994, 9, 23)),
            new UserData(81, "eliza_n", "eliza.n@example.com", "+40721000081", "https://example.com/avatars/81.jpg", "Loves horses and nature.", new DateTime(2002, 2, 16)),
            new UserData(82, "florin_o", "florin.o@example.com", "+40721000082", "https://example.com/avatars/82.jpg", "Loves basketball and BBQ.", new DateTime(1991, 7, 9)),
            new UserData(83, "georgiana_p", "georgiana.p@example.com", "+40721000083", "https://example.com/avatars/83.jpg", "Loves sushi and travel.", new DateTime(1997, 12, 2)),
            new UserData(84, "horatio_r", "horatio.r@example.com", "+40721000084", "https://example.com/avatars/84.jpg", "Sailor and adventurer.", new DateTime(1990, 5, 25)),
            new UserData(85, "iris_s", "iris.s@example.com", "+40721000085", "https://example.com/avatars/85.jpg", "Loves ballet and art.", new DateTime(2000, 10, 18)),
            new UserData(86, "julius_t", "julius.t@example.com", "+40721000086", "https://example.com/avatars/86.jpg", "Loves debate and politics.", new DateTime(1993, 3, 11)),
            new UserData(87, "karina_u", "karina.u@example.com", "+40721000087", "https://example.com/avatars/87.jpg", "Loves museums and history.", new DateTime(1998, 8, 4)),
            new UserData(88, "leon_v", "leon.v@example.com", "+40721000088", "https://example.com/avatars/88.jpg", "Loves camping and stars.", new DateTime(1995, 1, 27)),
            new UserData(89, "melisa_w", "melisa.w@example.com", "+40721000089", "https://example.com/avatars/89.jpg", "Blogger and coffee snob.", new DateTime(1999, 6, 20)),
            new UserData(90, "nelu_x", "nelu.x@example.com", "+40721000090", "https://example.com/avatars/90.jpg", "Farmer and nature lover.", new DateTime(1987, 11, 13)),
            new UserData(91, "ofelia_y", "ofelia.y@example.com", "+40721000091", "https://example.com/avatars/91.jpg", "Loves theater and books.", new DateTime(2001, 4, 6)),
            new UserData(92, "petre_z", "petre.z@example.com", "+40721000092", "https://example.com/avatars/92.jpg", "Taxi driver and chess fan.", new DateTime(1988, 9, 29)),
            new UserData(93, "quintina_a", "quintina.a@example.com", "+40721000093", "https://example.com/avatars/93.jpg", "Loves flowers and crafts.", new DateTime(1996, 2, 22)),
            new UserData(94, "remus_b", "remus.b@example.com", "+40721000094", "https://example.com/avatars/94.jpg", "Loves rugby and beer.", new DateTime(1992, 7, 15)),
            new UserData(95, "sabrina_c", "sabrina.c@example.com", "+40721000095", "https://example.com/avatars/95.jpg", "Loves spa days and yoga.", new DateTime(2000, 12, 8)),
            new UserData(96, "tiberiu_d", "tiberiu.d@example.com", "+40721000096", "https://example.com/avatars/96.jpg", "Loves rock music.", new DateTime(1994, 5, 1)),
            new UserData(97, "ulric_e", "ulric.e@example.com", "+40721000097", "https://example.com/avatars/97.jpg", "Loves board games.", new DateTime(1991, 10, 24)),
            new UserData(98, "vera_f", "vera.f@example.com", "+40721000098", "https://example.com/avatars/98.jpg", "Loves tennis and brunch.", new DateTime(1997, 3, 17)),
            new UserData(99, "winston_g", "winston.g@example.com", "+40721000099", "https://example.com/avatars/99.jpg", "Loves jazz and cycling.", new DateTime(1993, 8, 10)),
            new UserData(100, "xara_h", "xara_h@example.com", "+40721000100", "https://example.com/avatars/100.jpg", "Loves hiking and stargazing.", new DateTime(1998, 1, 3)),
            new UserData(101, "yolanda_i", "yolanda.i@example.com", "+40721000101", "https://example.com/avatars/101.jpg", "Loves cooking and Netflix.", new DateTime(2001, 6, 26)),
            new UserData(102, "zenon_j", "zenon.j@example.com", "+40721000102", "https://example.com/avatars/102.jpg", "Loves chess and jogging.", new DateTime(1990, 11, 19)),
            new UserData(103, "adela_k", "adela.k@example.com", "+40721000103", "https://example.com/avatars/103.jpg", "Loves painting and cats.", new DateTime(1995, 4, 12)),
            new UserData(104, "basil_l", "basil.l@example.com", "+40721000104", "https://example.com/avatars/104.jpg", "Loves cooking and travel.", new DateTime(1997, 9, 5)),
            new UserData(105, "celia_m", "celia.m@example.com", "+40721000105", "https://example.com/avatars/105.jpg", "Loves fashion and music.", new DateTime(2002, 2, 28)),
            new UserData(106, "dorin_n", "dorin.n@example.com", "+40721000106", "https://example.com/avatars/106.jpg", "Loves fishing and quiet.", new DateTime(1989, 7, 21)),
            new UserData(107, "estera_o", "estera.o@example.com", "+40721000107", "https://example.com/avatars/107.jpg", "Loves ceramics and tea.", new DateTime(1996, 12, 14)),
            new UserData(108, "faust_p", "faust.p@example.com", "+40721000108", "https://example.com/avatars/108.jpg", "Loves poetry and coffee.", new DateTime(1993, 5, 7)),
            new UserData(109, "geta_r", "geta.r@example.com", "+40721000109", "https://example.com/avatars/109.jpg", "Loves knitting and walks.", new DateTime(1999, 10, 30)),
            new UserData(110, "hugo_s", "hugo.s@example.com", "+40721000110", "https://example.com/avatars/110.jpg", "Loves boxing and movies.", new DateTime(1994, 3, 23)),
            new UserData(111, "ilinca_t", "ilinca.t@example.com", "+40721000111", "https://example.com/avatars/111.jpg", "Loves dancing and sunsets.", new DateTime(2000, 8, 16)),
            new UserData(112, "jorin_u", "jorin.u@example.com", "+40721000112", "https://example.com/avatars/112.jpg", "Loves gaming and pizza.", new DateTime(1997, 1, 9)),
            new UserData(113, "kosta_v", "kosta.v@example.com", "+40721000113", "https://example.com/avatars/113.jpg", "Loves travel and coffee.", new DateTime(1992, 6, 2)),
            new UserData(114, "luminita_w", "luminita.w@example.com", "+40721000114", "https://example.com/avatars/114.jpg", "Loves flowers and reading.", new DateTime(1998, 11, 25)),
            new UserData(115, "marian_x", "marian.x@example.com", "+40721000115", "https://example.com/avatars/115.jpg", "Loves hiking and history.", new DateTime(1991, 4, 18)),
        };

        public UserData GetUserData(int userId)
        {
            UserData result = null;
            foreach (UserData ud in users)
            {
                if(ud.Id == userId) { result = ud; break; }
            }
            if(result == null)
            {
                throw new Exception("User not found");
            }
            return result;
        }
    }
}
