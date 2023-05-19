// ported from longmynd - https://github.com/myorangedragon/longmynd - Heather Lomond

namespace opentuner.Hardware
{
    public static class stv0910_regs_init
    {
        public struct STReg
        {
            public ushort reg;
            public byte val;

            public STReg(ushort _reg, byte _val)
            {
                reg = _reg;
                val = _val;
            }
        }

        public static STReg[] STV0910DefVal = new STReg[]
        {
 /* SYS registers */
/*  new STReg( stv0910_regs.RSTV0910_MID,               0x51 ),    MID              R only */
/*  new STReg( stv0910_regs.RSTV0910_DID,               0x20 ),    DID              R only */
    new STReg( stv0910_regs.RSTV0910_DACR1,             0x00 ), /* DACR1            DAC 1 freq=0, mode=0 */
    new STReg( stv0910_regs.RSTV0910_DACR2,             0x00 ), /* DACR2            DAC 2 freq=0, mode=0 */
    new STReg( stv0910_regs.RSTV0910_PADCFG,            0x05 ), /* PADCFG           AGCRF_1 inverted, Push-pull, AGCRF_2 same*/
///    new STReg( stv0910_regs.RSTV0910_OUTCFG2,           0x00 ), /* OUTCFG2          all Transport stream signals not inverted  */
///    new STReg( stv0910_regs.RSTV0910_OUTCFG2,           0x44 ), /* OUTCFG2    invert VALID (DPN) */
///    new STReg( stv0910_regs.RSTV0910_OUTCFG2,           0x55 ), /* OUTCFG2    invert VALID and CLOCK */
///    new STReg( stv0910_regs.RSTV0910_OUTCFG2,           0x11 ), /* OUTCFG2    CLOCK */
    new STReg( stv0910_regs.RSTV0910_OUTCFG2,           0x00 ),
    new STReg( stv0910_regs.RSTV0910_OUTCFG,            0x00 ), /* OUTCFG           TS2 serial pins push-pull, ts1 serial pins push-pull, /                                                                 ts2 parallel pins push-pull, ts1 parallel pins push-pull 
                                                              serial data output is D7 */
    new STReg( stv0910_regs.RSTV0910_IRQSTATUS3,        0x00 ), /* IRQSTATUS3       reset all pending IRQs */
    new STReg( stv0910_regs.RSTV0910_IRQSTATUS2,        0x00 ), /* IRQSTATUS2       reset all pending IRQs */
    new STReg( stv0910_regs.RSTV0910_IRQSTATUS1,        0x00 ), /* IRQSTATUS1       reset all pending IRQs */
    new STReg( stv0910_regs.RSTV0910_IRQSTATUS0,        0x00 ), /* IRQSTATUS0       reset all pending IRQs */
    new STReg( stv0910_regs.RSTV0910_IRQMASK3,          0x00 ), /* IRQMASK3         disable all IRQs */
    new STReg( stv0910_regs.RSTV0910_IRQMASK2,          0x00 ), /* IRQMASK2         disable all IRQs*/
    new STReg( stv0910_regs.RSTV0910_IRQMASK1,          0x00 ), /* IRQMASK1         disable all IRQs*/
    new STReg( stv0910_regs.RSTV0910_IRQMASK0,          0x00 ), /* IRQMASK0         disable all IRQs*/
    new STReg( stv0910_regs.RSTV0910_I2CCFG,            0x88 ), /* I2CCFG           i2c fastmode enabled, auto inc by 1 */
    new STReg( stv0910_regs.RSTV0910_P1_I2CRPT,         0x38 ), /* P1_I2CRPT        was 0x34, updated to 0x38
                                                                 P1 repeater off, repeater speed =135/32Mhz, delay on SCL, manual stop
                                                                 bidirectional SDAT2SDA */
    new STReg( stv0910_regs.RSTV0910_P2_I2CRPT,         0x38 ), /* P2_I2CRPT        P2 as above  */
    new STReg( stv0910_regs.RSTV0910_GPIO0CFG,          0x82 ), /* GPIO0CFG         GPIO 0 push-pull  iconfig = 0x01 (force to 1),  non inverting */ 
    new STReg( stv0910_regs.RSTV0910_GPIO1CFG,          0x82 ), /* GPIO1CFG              1 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO2CFG,          0x82 ), /* GPIO2CFG              2 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO3CFG,          0x82 ), /* GPIO3CFG              3 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO4CFG,          0x82 ), /* GPIO4CFG              4 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO5CFG,          0x82 ), /* GPIO5CFG         was 0x04, updated to 0x82 */
    new STReg( stv0910_regs.RSTV0910_GPIO6CFG,          0x82 ), /* GPIO6CFG         was 0x06, updated to 0x82 */
    new STReg( stv0910_regs.RSTV0910_GPIO7CFG,          0x82 ), /* GPIO7CFG              7 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO8CFG,          0x82 ), /* GPIO8CFG              8 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO9CFG,          0x82 ), /* GPIO9CFG              9 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO10CFG,         0x82 ), /* GPIO10CFG             10 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO11CFG,         0x82 ), /* GPIO11CFG             11 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO12CFG,         0x82 ), /* GPIO12CFG             12 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO13CFG,         0x82 ), /* GPIO13CFG             13 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO14CFG,         0x82 ), /* GPIO14CFG             14 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO15CFG,         0x82 ), /* GPIO15CFG             15 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO16CFG,         0x82 ), /* GPIO16CFG             16 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO17CFG,         0x82 ), /* GPIO17CFG             17 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO18CFG,         0x82 ), /* GPIO18CFG             18 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO19CFG,         0x82 ), /* GPIO19CFG             19 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO20CFG,         0x82 ), /* GPIO20CFG             20 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO21CFG,         0x82 ), /* GPIO21CFG             21 "  */
    new STReg( stv0910_regs.RSTV0910_GPIO22CFG,         0x82 ), /* GPIO22CFG             22 "  */
    new STReg( stv0910_regs.RSTV0910_STRSTATUS1,        0x60 ), /* STRSTATUS1       was 0x93, updated to 0x60
                                                                 gpio2=demod 2 detect iq inversion,
                                                                 gpio1=demod 1 detect iq inversion   */
    new STReg( stv0910_regs.RSTV0910_STRSTATUS2,        0x71 ), /* STRSTATUS2       gpio4=demod 2 lock,                gpio3=demod 1 lock */
    new STReg( stv0910_regs.RSTV0910_STRSTATUS3,        0x82 ), /* STRSTATUS3       gpio6=demod 2 failed flag,         gpio5=demod 1 failed flag  */

    /* FSK Registers */
    new STReg( stv0910_regs.RSTV0910_FSKTFC2,           0x8c ), /* FSKTFC2          FSK is not being used so can ignore these */
    new STReg( stv0910_regs.RSTV0910_FSKTFC1,           0x45 ), /* FSKTFC1          */
    new STReg( stv0910_regs.RSTV0910_FSKTFC0,           0xc9 ), /* FSKTFC0          */
    new STReg( stv0910_regs.RSTV0910_FSKTDELTAF1,       0x01 ), /* FSKTDELTAF1      */
    new STReg( stv0910_regs.RSTV0910_FSKTDELTAF0,       0x37 ), /* FSKTDELTAF0      */
    new STReg( stv0910_regs.RSTV0910_FSKTCTRL,          0x08 ), /* FSKTCTRL         modulator on when FSKTX_EN=1,
                                                                 FSKTX_OUT=Fc-df  (FSKTX_IN=0) = fc+df (FSKTX_IN=1),  
                                                                 modulator enabled by FSKTX_EN input,
                                                                 fsk modulator output second order sig-delta */
    new STReg( stv0910_regs.RSTV0910_FSKRFC2,           0x10 ), /* FSKRFC2          */
    new STReg( stv0910_regs.RSTV0910_FSKRFC1,           0x45 ), /* FSKRFC1          */
    new STReg( stv0910_regs.RSTV0910_FSKRFC0,           0xc9 ), /* FSKRFC0          */
    new STReg( stv0910_regs.RSTV0910_FSKRK1,            0x38 ), /* FSKRK1           */
    new STReg( stv0910_regs.RSTV0910_FSKRK2,            0x71 ), /* FSKRK2           */
    new STReg( stv0910_regs.RSTV0910_FSKRAGCR,          0x28 ), /* FSKRAGCR         */
/*  new STReg( stv0910_regs.RSTV0910_FSKRAGC,           0xff ),    FSKRAGC          R only */
    new STReg( stv0910_regs.RSTV0910_FSKRALPHA,         0x13 ), /* FSKRALPHA        */
    new STReg( stv0910_regs.RSTV0910_FSKRPLTH1,         0x90 ), /* FSKRPLTH1        */
    new STReg( stv0910_regs.RSTV0910_FSKRPLTH0,         0xbe ), /* FSKRPLTH0        */
/*  new STReg( stv0910_regs.RSTV0910_FSKRDF1,           0x80 ),    FSKRDF1          R only */
/*  new STReg( stv0910_regs.RSTV0910_FSKRDF0,           0x00 ),    FSKRDF0          R Only */
    new STReg( stv0910_regs.RSTV0910_FSKRSTEPP,         0x58 ), /* FSKRSTEPP        */
    new STReg( stv0910_regs.RSTV0910_FSKRSTEPM,         0x6f ), /* FSKRSTEPM        */
/*  new STReg( stv0910_regs.RSTV0910_FSKRDET1,          0x00 ),    FSKRDET1         R only */
/*  new STReg( stv0910_regs.RSTV0910_FSKRDET0,          0x01 ),    FSKRDET0         R only */
    new STReg( stv0910_regs.RSTV0910_FSKRDTH1,          0x00 ), /* FSKRDTH1         */
    new STReg( stv0910_regs.RSTV0910_FSKRDTH0,          0xe9 ), /* FSKRDTH0         */
    new STReg( stv0910_regs.RSTV0910_FSKRLOSS,          0x4d ), /* FSKRLOSS         */

    /* clocks and power registers */
    new STReg( stv0910_regs.RSTV0910_NCOARSE,           0x39 ), /* NCOARSE          charge pump (CP)=7 (for NDIV from 8 to 71), IDF=1 */ 
                                          /*                  f_mclk2 = 270 MHz, = f_ref * n_div / IDF / 2. f_ref=xtal=30MHz */
    new STReg( stv0910_regs.RSTV0910_NCOARSE1,          0x12 ), /* NCOARSE1         N_DIV = 0x12 */
    new STReg( stv0910_regs.RSTV0910_NCOARSE2,          0x04 ), /* NCOARSE2         ODF=0x4 f_ana = 135MHz =  f_ref * n_div / IDF / ODF*/
    new STReg( stv0910_regs.RSTV0910_SYNTCTRL,          0xc2 ), /* SYNTCTRL         was 0x02, updated to 0xc2
                                                                 stop all clocks except i2c, bypass pll, pll active, osc pad enabled */
    new STReg( stv0910_regs.RSTV0910_FILTCTRL,          0x01 ), /* FILTCTRL         filter FSK clock not inverted, if PLL bypassed, clock from CLK1 only */
    new STReg( stv0910_regs.RSTV0910_PLLSTAT,           0x07 ), /* PLLSTAT          LSB is PLL lock */
    new STReg( stv0910_regs.RSTV0910_STOPCLK1,          0x00 ), /* STOPCLK1         neither ADC interface clocks are inverted */
    new STReg( stv0910_regs.RSTV0910_STOPCLK2,          0x00 ), /* STOPCLK2         no clocks stopped */
    new STReg( stv0910_regs.RSTV0910_PREGCTL,           0x00 ), /* PREGCTL          DCDC 3v3 to 2v5 on */
    new STReg( stv0910_regs.RSTV0910_TSTTNR0,           0x00 ), /* TSTTNR0          was 0x04 updated to  0x00
                                                                 FSK analog cell off */

    // ***
    new STReg( stv0910_regs.RSTV0910_TSTTNR1,           0x44 ),  //TSTTNR1          was 0x46 updated to 0x44
                                                                 //ADC1 power off. note reset=0x26, upper bits are reserved 
    new STReg( stv0910_regs.RSTV0910_TSTTNR1,           0x46 ), // TSTTNR1           ADC1 power on		(~!~!~)
    new STReg( stv0910_regs.RSTV0910_TSTTNR2,           0x4b ), // TSTTNR2          was 0x6b updated to 0x4b
///                                                                 I2C DiSEqC ADC 1 power off, diseqc clock div = 0xb
///                                                                 f_diseqc = 135MHz/2*(diseq_clk_div+17) = 2.41MHz y
    new STReg( stv0910_regs.RSTV0910_TSTTNR3,           0x46 ), /* TSTTNR3          ADC2 power on. note again reset=0x26 again 0x46 writes to reserved  */
    // ***

    /* DMD P2 Registers */
    new STReg( stv0910_regs.RSTV0910_P2_IQCONST,        0x00 ), /* P2_IQCONST */
    new STReg( stv0910_regs.RSTV0910_P2_NOSCFG,   0x20 +      0x14 ), /* P2_NOSCFG */
    new STReg( stv0910_regs.RSTV0910_P2_ISYMB,          0x0e ), /* P2_ISYMB */
    new STReg( stv0910_regs.RSTV0910_P2_QSYMB,          0xfc ), /* P2_QSYMB */
    new STReg( stv0910_regs.RSTV0910_P2_AGC1CFG,        0x54 ), /* P2_AGC1CFG */
    new STReg( stv0910_regs.RSTV0910_P2_AGC1CN,         0x99 ), /* P2_AGC1CN */
    new STReg( stv0910_regs.RSTV0910_P2_AGC1REF,        0x58 ), /* P2_AGC1REF */
    new STReg( stv0910_regs.RSTV0910_P2_IDCCOMP,        0x0a ), /* P2_IDCCOMP */
    new STReg( stv0910_regs.RSTV0910_P2_QDCCOMP,        0x09 ), /* P2_QDCCOMP */
    new STReg( stv0910_regs.RSTV0910_P2_POWERI,         0x09 ), /* P2_POWERI */
    new STReg( stv0910_regs.RSTV0910_P2_POWERQ,         0x0a ), /* P2_POWERQ */
    new STReg( stv0910_regs.RSTV0910_P2_AGC1AMM,        0xfd ), /* P2_AGC1AMM */
    new STReg( stv0910_regs.RSTV0910_P2_AGC1QUAD,       0xfd ), /* P2_AGC1QUAD */
    new STReg( stv0910_regs.RSTV0910_P2_AGCIQIN1,       0x00 ), /* P2_AGCIQIN1 */
    new STReg( stv0910_regs.RSTV0910_P2_AGCIQIN0,       0x00 ), /* P2_AGCIQIN0 */
    new STReg( stv0910_regs.RSTV0910_P2_DEMOD,          0x00 ), /* P2_DEMOD */
    new STReg( stv0910_regs.RSTV0910_P2_DMDMODCOD,      0x10 ), /* P2_DMDMODCOD */
    new STReg( stv0910_regs.RSTV0910_P2_DSTATUS,        0x10 ), /* P2_DSTATUS */
    new STReg( stv0910_regs.RSTV0910_P2_DSTATUS2,       0x80 ), /* P2_DSTATUS2 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDCFGMD,       0xc9 ), /* P2_DMDCFGMD */
    new STReg( stv0910_regs.RSTV0910_P2_DMDCFG2,        0x3b ), /* P2_DMDCFG2 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDISTATE,      0x5c ), /* P2_DMDISTATE */
    new STReg( stv0910_regs.RSTV0910_P2_DMDT0M,         0x40 ), /* P2_DMDT0M */
    new STReg( stv0910_regs.RSTV0910_P2_DMDSTATE,       0x1c ), /* P2_DMDSTATE */
    new STReg( stv0910_regs.RSTV0910_P2_DMDFLYW,        0x00 ), /* P2_DMDFLYW */
    new STReg( stv0910_regs.RSTV0910_P2_DSTATUS3,       0x80 ), /* P2_DSTATUS3 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDCFG3,        0x08 ), /* P2_DMDCFG3 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDCFG4,        0x04 ), /* P2_DMDCFG4 */
    new STReg( stv0910_regs.RSTV0910_P2_CORRELMANT,     0x78 ), /* P2_CORRELMANT */
    new STReg( stv0910_regs.RSTV0910_P2_CORRELABS,      0x8C ), /* P2_CORRELABS */
    new STReg( stv0910_regs.RSTV0910_P2_CORRELEXP,      0xaa ), /* P2_CORRELEXP */
    new STReg( stv0910_regs.RSTV0910_P2_PLHMODCOD,      0x10 ), /* P2_PLHMODCOD */
    new STReg( stv0910_regs.RSTV0910_P2_DMDREG,         0x01 ), /* P2_DMDREG */
    new STReg( stv0910_regs.RSTV0910_P2_AGCNADJ,        0x00 ), /* P2_AGCNADJ */
    new STReg( stv0910_regs.RSTV0910_P2_AGCKS,          0x00 ), /* P2_AGCKS */
    new STReg( stv0910_regs.RSTV0910_P2_AGCKQ,          0x00 ), /* P2_AGCKQ */
    new STReg( stv0910_regs.RSTV0910_P2_AGCK8,          0x00 ), /* P2_AGCK8 */
    new STReg( stv0910_regs.RSTV0910_P2_AGCK16,         0x00 ), /* P2_AGCK16 */
    new STReg( stv0910_regs.RSTV0910_P2_AGCK32,         0x00 ), /* P2_AGCK32 */
    new STReg( stv0910_regs.RSTV0910_P2_AGC2O,          0x5b ), /* P2_AGC2O */
    new STReg( stv0910_regs.RSTV0910_P2_AGC2REF,        0x38 ), /* P2_AGC2REF */
    new STReg( stv0910_regs.RSTV0910_P2_AGC1ADJ,        0x58 ), /* P2_AGC1ADJ */
    new STReg( stv0910_regs.RSTV0910_P2_AGCRSADJ,       0x38 ), /* P2_AGCRSADJ */
    new STReg( stv0910_regs.RSTV0910_P2_AGCRQADJ,       0x38 ), /* P2_AGCRQADJ */
    new STReg( stv0910_regs.RSTV0910_P2_AGCR8ADJ,       0x38 ), /* P2_AGCR8ADJ */
    new STReg( stv0910_regs.RSTV0910_P2_AGCR1ADJ,       0x38 ), /* P2_AGCR1ADJ */
    new STReg( stv0910_regs.RSTV0910_P2_AGCR2ADJ,       0x38 ), /* P2_AGCR2ADJ */
    new STReg( stv0910_regs.RSTV0910_P2_AGCR3ADJ,       0x47 ), /* P2_AGCR3ADJ */
    new STReg( stv0910_regs.RSTV0910_P2_AGCREFADJ,      0x38 ), /* P2_AGCREFADJ */
    new STReg( stv0910_regs.RSTV0910_P2_AGC2I1,         0x1c ), /* P2_AGC2I1 */
    new STReg( stv0910_regs.RSTV0910_P2_AGC2I0,         0x74 ), /* P2_AGC2I0 */
    new STReg( stv0910_regs.RSTV0910_P2_CARCFG,         0x46 ), /* P2_CARCFG */
    new STReg( stv0910_regs.RSTV0910_P2_ACLC,           0x2b ), /* P2_ACLC */
    new STReg( stv0910_regs.RSTV0910_P2_BCLC,           0x1a ), /* P2_BCLC */
    new STReg( stv0910_regs.RSTV0910_P2_ACLCS2,         0x00 ), /* P2_ACLCS2 */
    new STReg( stv0910_regs.RSTV0910_P2_BCLCS2,         0x00 ), /* P2_BCLCS2 */
    new STReg( stv0910_regs.RSTV0910_P2_CARFREQ,        0x79 ), /* P2_CARFREQ */
    new STReg( stv0910_regs.RSTV0910_P2_CARHDR,         0x1c ), /* P2_CARHDR */
    new STReg( stv0910_regs.RSTV0910_P2_LDT,            0xd0 ), /* P2_LDT */
    new STReg( stv0910_regs.RSTV0910_P2_LDT2,           0xb8 ), /* P2_LDT2 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRICFG,        0xf9 ), /* P2_CFRICFG */
    new STReg( stv0910_regs.RSTV0910_P2_CFRUP1,         0x0e ), /* P2_CFRUP1 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRUP0,         0x69 ), /* P2_CFRUP0 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRIBASE1,      0x01 ), /* P2_CFRIBASE1 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRIBASE0,      0xf5 ), /* P2_CFRIBASE0 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRLOW1,        0xf1 ), /* P2_CFRLOW1 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRLOW0,        0x97 ), /* P2_CFRLOW0 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRINIT1,       0x01 ), /* P2_CFRINIT1 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRINIT0,       0xf5 ), /* P2_CFRINIT0 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRINC1,        0x03 ), /* P2_CFRINC1 */
    new STReg( stv0910_regs.RSTV0910_P2_CFRINC0,        0x8e ), /* P2_CFRINC0 */
    new STReg( stv0910_regs.RSTV0910_P2_CFR2,           0x01 ), /* P2_CFR2 */
    new STReg( stv0910_regs.RSTV0910_P2_CFR1,           0xf5 ), /* P2_CFR1 */
    new STReg( stv0910_regs.RSTV0910_P2_CFR0,           0x00 ), /* P2_CFR0 */
    new STReg( stv0910_regs.RSTV0910_P2_LDI,            0xb6 ), /* P2_LDI */
    new STReg( stv0910_regs.RSTV0910_P2_TMGCFG,         0xd3 ), /* P2_TMGCFG */
    new STReg( stv0910_regs.RSTV0910_P2_RTC,            0x68 ), /* P2_RTC */
    new STReg( stv0910_regs.RSTV0910_P2_RTCS2,          0x68 ), /* P2_RTCS2 */
    new STReg( stv0910_regs.RSTV0910_P2_TMGTHRISE,      0x1e ), /* P2_TMGTHRISE */
    new STReg( stv0910_regs.RSTV0910_P2_TMGTHFALL,      0x08 ), /* P2_TMGTHFALL */
    new STReg( stv0910_regs.RSTV0910_P2_SFRUPRATIO,     0x20 ), /* P2_SFRUPRATIO */
    new STReg( stv0910_regs.RSTV0910_P2_SFRLOWRATIO,    0xd0 ), /* P2_SFRLOWRATIO */
    new STReg( stv0910_regs.RSTV0910_P2_KTTMG,          0xa0 ), /* P2_KTTMG */
    new STReg( stv0910_regs.RSTV0910_P2_KREFTMG,        0x80 ), /* P2_KREFTMG */
    new STReg( stv0910_regs.RSTV0910_P2_SFRSTEP,        0x88 ), /* P2_SFRSTEP */
    new STReg( stv0910_regs.RSTV0910_P2_TMGCFG2,        0x80 ), /* P2_TMGCFG2 */
    new STReg( stv0910_regs.RSTV0910_P2_KREFTMG2,       0x80 ), /* P2_KREFTMG2 */
    new STReg( stv0910_regs.RSTV0910_P2_TMGCFG3,        0x06 ), /* P2_TMGCFG3 */
    new STReg( stv0910_regs.RSTV0910_P2_SFRINIT1,       0x38 ), /* P2_SFRINIT1 */
    new STReg( stv0910_regs.RSTV0910_P2_SFRINIT0,       0xe3 ), /* P2_SFRINIT0 */
    new STReg( stv0910_regs.RSTV0910_P2_SFRUP1,         0x3f ), /* P2_SFRUP1 */
    new STReg( stv0910_regs.RSTV0910_P2_SFRUP0,         0xff ), /* P2_SFRUP0 */
    new STReg( stv0910_regs.RSTV0910_P2_SFRLOW1,        0x2e ), /* P2_SFRLOW1 */
    new STReg( stv0910_regs.RSTV0910_P2_SFRLOW0,        0x39 ), /* P2_SFRLOW0 */
    new STReg( stv0910_regs.RSTV0910_P2_SFR3,           0x38 ), /* P2_SFR3 */
    new STReg( stv0910_regs.RSTV0910_P2_SFR2,           0xe3 ), /* P2_SFR2 */
    new STReg( stv0910_regs.RSTV0910_P2_SFR1,           0x00 ), /* P2_SFR1 */
    new STReg( stv0910_regs.RSTV0910_P2_SFR0,           0x00 ), /* P2_SFR0 */
    new STReg( stv0910_regs.RSTV0910_P2_TMGREG2,        0x00 ), /* P2_TMGREG2 */
    new STReg( stv0910_regs.RSTV0910_P2_TMGREG1,        0x00 ), /* P2_TMGREG1 */
    new STReg( stv0910_regs.RSTV0910_P2_TMGREG0,        0x00 ), /* P2_TMGREG0 */
    new STReg( stv0910_regs.RSTV0910_P2_TMGLOCK1,       0xe4 ), /* P2_TMGLOCK1 */
    new STReg( stv0910_regs.RSTV0910_P2_TMGLOCK0,       0x00 ), /* P2_TMGLOCK0 */
    new STReg( stv0910_regs.RSTV0910_P2_TMGOBS,         0x10 ), /* P2_TMGOBS */
    new STReg( stv0910_regs.RSTV0910_P2_EQUALCFG,       0x41 ), /* P2_EQUALCFG */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAI1,         0x00 ), /* P2_EQUAI1 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAQ1,         0x00 ), /* P2_EQUAQ1 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAI2,         0x00 ), /* P2_EQUAI2 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAQ2,         0x00 ), /* P2_EQUAQ2 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAI3,         0x00 ), /* P2_EQUAI3 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAQ3,         0x00 ), /* P2_EQUAQ3 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAI4,         0x00 ), /* P2_EQUAI4 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAQ4,         0x00 ), /* P2_EQUAQ4 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAI5,         0x00 ), /* P2_EQUAI5 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAQ5,         0x00 ), /* P2_EQUAQ5 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAI6,         0x00 ), /* P2_EQUAI6 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAQ6,         0x00 ), /* P2_EQUAQ6 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAI7,         0x00 ), /* P2_EQUAI7 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAQ7,         0x00 ), /* P2_EQUAQ7 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAI8,         0x00 ), /* P2_EQUAI8 */
    new STReg( stv0910_regs.RSTV0910_P2_EQUAQ8,         0x00 ), /* P2_EQUAQ8 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSDATAT1,     0xff ), /* P2_NNOSDATAT1 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSDATAT0,     0xff ), /* P2_NNOSDATAT0 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSDATA1,      0xff ), /* P2_NNOSDATA1 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSDATA0,      0xff ), /* P2_NNOSDATA0 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSPLHT1,      0xff ), /* P2_NNOSPLHT1 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSPLHT0,      0xff ), /* P2_NNOSPLHT0 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSPLH1,       0xff ), /* P2_NNOSPLH1 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSPLH0,       0xff ), /* P2_NNOSPLH0 */
    new STReg( stv0910_regs.RSTV0910_P2_NOSDATAT1,      0xff ), /* P2_NOSDATAT1 */
    new STReg( stv0910_regs.RSTV0910_P2_NOSDATAT0,      0xff ), /* P2_NOSDATAT0 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSFRAME1,     0xff ), /* P2_NNOSFRAME1 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSFRAME0,     0xff ), /* P2_NNOSFRAME0 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSRAD1,       0x00 ), /* P2_NNOSRAD1 */
    new STReg( stv0910_regs.RSTV0910_P2_NNOSRAD0,       0x00 ), /* P2_NNOSRAD0 */
    new STReg( stv0910_regs.RSTV0910_P2_NOSCFGF1,       0x00 ), /* P2_NOSCFGF1 */
    new STReg( stv0910_regs.RSTV0910_P2_NOSCFGF2,       0x00 ), /* P2_NOSCFGF2 */
    new STReg( stv0910_regs.RSTV0910_P2_CAR2CFG,        0x06 ), /* P2_CAR2CFG */
    new STReg( stv0910_regs.RSTV0910_P2_CFR2CFR1,       0xe5 ), /* P2_CFR2CFR1 */
    new STReg( stv0910_regs.RSTV0910_P2_CAR3CFG,        0x02 ), /* P2_CAR3CFG */
    new STReg( stv0910_regs.RSTV0910_P2_CFR22,          0x00 ), /* P2_CFR22 */
    new STReg( stv0910_regs.RSTV0910_P2_CFR21,          0x00 ), /* P2_CFR21 */
    new STReg( stv0910_regs.RSTV0910_P2_CFR20,          0x00 ), /* P2_CFR20 */
    new STReg( stv0910_regs.RSTV0910_P2_ACLC2S2Q,       0x0b ), /* P2_ACLC2S2Q */
    new STReg( stv0910_regs.RSTV0910_P2_ACLC2S28,       0x0a ), /* P2_ACLC2S28 */
    new STReg( stv0910_regs.RSTV0910_P2_ACLC2S216A,     0x49 ), /* P2_ACLC2S216A */
    new STReg( stv0910_regs.RSTV0910_P2_ACLC2S232A,     0x48 ), /* P2_ACLC2S232A */
    new STReg( stv0910_regs.RSTV0910_P2_BCLC2S2Q,       0x84 ), /* P2_BCLC2S2Q */
    new STReg( stv0910_regs.RSTV0910_P2_BCLC2S28,       0x84 ), /* P2_BCLC2S28 */
    new STReg( stv0910_regs.RSTV0910_P2_BCLC2S216A,     0x84 ), /* P2_BCLC2S216A */
    new STReg( stv0910_regs.RSTV0910_P2_BCLC2S232A,     0x84 ), /* P2_BCLC2S232A */
    new STReg( stv0910_regs.RSTV0910_P2_PLROOT2,        0x00 ), /* P2_PLROOT2 */
    new STReg( stv0910_regs.RSTV0910_P2_PLROOT1,        0x00 ), /* P2_PLROOT1 */
    new STReg( stv0910_regs.RSTV0910_P2_PLROOT0,        0x01 ), /* P2_PLROOT0 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST0,     0xff ), /* P2_MODCODLST0 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST1,     0xfc ), /* P2_MODCODLST1 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST2,     0x00 ), /* P2_MODCODLST2 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST3,     0x00 ), /* P2_MODCODLST3 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST4,     0x00 ), /* P2_MODCODLST4 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST5,     0x00 ), /* P2_MODCODLST5 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST6,     0x00 ), /* P2_MODCODLST6 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST7,     0xc0 ), /* P2_MODCODLST7 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST8,     0x00 ), /* P2_MODCODLST8 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLST9,     0x00 ), /* P2_MODCODLST9 */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLSTA,     0xc0 ), /* P2_MODCODLSTA */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLSTB,     0x00 ), /* P2_MODCODLSTB */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLSTC,     0x00 ), /* P2_MODCODLSTC */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLSTD,     0x00 ), /* P2_MODCODLSTD */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLSTE,     0x00 ), /* P2_MODCODLSTE */
    new STReg( stv0910_regs.RSTV0910_P2_MODCODLSTF,     0x0f ), /* P2_MODCODLSTF */
    new STReg( stv0910_regs.RSTV0910_P2_GAUSSR0,        0x98 ), /* P2_GAUSSR0 */
    new STReg( stv0910_regs.RSTV0910_P2_CCIR0,          0x30 ), /* P2_CCIR0 */
    new STReg( stv0910_regs.RSTV0910_P2_CCIQUANT,       0xac ), /* P2_CCIQUANT */
    new STReg( stv0910_regs.RSTV0910_P2_CCITHRES,       0x50 ), /* P2_CCITHRES */
    new STReg( stv0910_regs.RSTV0910_P2_CCIACC,         0x00 ), /* P2_CCIACC */
    new STReg( stv0910_regs.RSTV0910_P2_DSTATUS4,       0x64 ), /* P2_DSTATUS4 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESCFG,      0x29 ), /* P2_DMDRESCFG */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESADR,      0x00 ), /* P2_DMDRESADR */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESDATA7,    0x00 ), /* P2_DMDRESDATA7 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESDATA6,    0x00 ), /* P2_DMDRESDATA6 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESDATA5,    0x00 ), /* P2_DMDRESDATA5 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESDATA4,    0x00 ), /* P2_DMDRESDATA4 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESDATA3,    0x00 ), /* P2_DMDRESDATA3 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESDATA2,    0x00 ), /* P2_DMDRESDATA2 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESDATA1,    0x00 ), /* P2_DMDRESDATA1 */
    new STReg( stv0910_regs.RSTV0910_P2_DMDRESDATA0,    0x00 ), /* P2_DMDRESDATA0 */
    new STReg( stv0910_regs.RSTV0910_P2_FFEI1,          0x00 ), /* P2_FFEI1 */
    new STReg( stv0910_regs.RSTV0910_P2_FFEQ1,          0x00 ), /* P2_FFEQ1 */
    new STReg( stv0910_regs.RSTV0910_P2_FFEI2,          0x00 ), /* P2_FFEI2 */
    new STReg( stv0910_regs.RSTV0910_P2_FFEQ2,          0x00 ), /* P2_FFEQ2 */
    new STReg( stv0910_regs.RSTV0910_P2_FFEI3,          0x00 ), /* P2_FFEI3 */
    new STReg( stv0910_regs.RSTV0910_P2_FFEQ3,          0x00 ), /* P2_FFEQ3 */
    new STReg( stv0910_regs.RSTV0910_P2_FFEI4,          0x00 ), /* P2_FFEI4 */
    new STReg( stv0910_regs.RSTV0910_P2_FFEQ4,          0x00 ), /* P2_FFEQ4 */
    new STReg( stv0910_regs.RSTV0910_P2_FFECFG,         0x71 ), /* P2_FFECFG */
    new STReg( stv0910_regs.RSTV0910_P2_TNRCFG2,        0x02 ), /* P2_TNRCFG2 */
    new STReg( stv0910_regs.RSTV0910_P2_SMAPCOEF7,      0x00 ), /* P2_SMAPCOEF7 */
    new STReg( stv0910_regs.RSTV0910_P2_SMAPCOEF6,      0x00 ), /* P2_SMAPCOEF6 */
    new STReg( stv0910_regs.RSTV0910_P2_SMAPCOEF5,      0x00 ), /* P2_SMAPCOEF5 */
    new STReg( stv0910_regs.RSTV0910_P2_SMAPCOEF4,      0x00 ), /* P2_SMAPCOEF4 */
    new STReg( stv0910_regs.RSTV0910_P2_SMAPCOEF3,      0x00 ), /* P2_SMAPCOEF3 */
    new STReg( stv0910_regs.RSTV0910_P2_SMAPCOEF2,      0x00 ), /* P2_SMAPCOEF2 */
    new STReg( stv0910_regs.RSTV0910_P2_SMAPCOEF1,      0x00 ), /* P2_SMAPCOEF1 */
    new STReg( stv0910_regs.RSTV0910_P2_SMAPCOEF0,      0x71 ), /* P2_SMAPCOEF0 */
    new STReg( stv0910_regs.RSTV0910_P2_NOSTHRES1,      0x60 ), /* P2_NOSTHRES1 */
    new STReg( stv0910_regs.RSTV0910_P2_NOSTHRES2,      0x69 ), /* P2_NOSTHRES2 */
    new STReg( stv0910_regs.RSTV0910_P2_NOSDIFF1,       0x80 ), /* P2_NOSDIFF1 */
    new STReg( stv0910_regs.RSTV0910_P2_RAINFADE,       0x35 ), /* P2_RAINFADE */
    new STReg( stv0910_regs.RSTV0910_P2_NOSRAMCFG,      0x28 ), /* P2_NOSRAMCFG */
    new STReg( stv0910_regs.RSTV0910_P2_NOSRAMPOS,      0x26 ), /* P2_NOSRAMPOS */
    new STReg( stv0910_regs.RSTV0910_P2_NOSRAMVAL,      0x86 ), /* P2_NOSRAMVAL */
    new STReg( stv0910_regs.RSTV0910_P2_DMDPLHSTAT,     0x00 ), /* P2_DMDPLHSTAT */
    new STReg( stv0910_regs.RSTV0910_P2_LOCKTIME3,      0xc0 ), /* P2_LOCKTIME3 */
    new STReg( stv0910_regs.RSTV0910_P2_LOCKTIME2,      0xd2 ), /* P2_LOCKTIME2 */
    new STReg( stv0910_regs.RSTV0910_P2_LOCKTIME1,      0x3b ), /* P2_LOCKTIME1 */
    new STReg( stv0910_regs.RSTV0910_P2_LOCKTIME0,      0x46 ), /* P2_LOCKTIME0 */
    /* DVB1 P2 Registers */
    new STReg( stv0910_regs.RSTV0910_P2_VITSCALE,       0x80 ), /* P2_VITSCALE */
    new STReg( stv0910_regs.RSTV0910_P2_FECM,           0x00 ), /* P2_FECM */
    new STReg( stv0910_regs.RSTV0910_P2_VTH12,          0xd7 ), /* P2_VTH12 */
    new STReg( stv0910_regs.RSTV0910_P2_VTH23,          0x85 ), /* P2_VTH23 */
    new STReg( stv0910_regs.RSTV0910_P2_VTH34,          0x58 ), /* P2_VTH34 */
    new STReg( stv0910_regs.RSTV0910_P2_VTH56,          0x3a ), /* P2_VTH56 */
    new STReg( stv0910_regs.RSTV0910_P2_VTH67,          0x34 ), /* P2_VTH67 */
    new STReg( stv0910_regs.RSTV0910_P2_VTH78,          0x28 ), /* P2_VTH78 */
    new STReg( stv0910_regs.RSTV0910_P2_VITCURPUN,      0x0d ), /* P2_VITCURPUN */
    new STReg( stv0910_regs.RSTV0910_P2_VERROR,         0xff ), /* P2_VERROR */
    new STReg( stv0910_regs.RSTV0910_P2_PRVIT,          0x2f ), /* P2_PRVIT */
    new STReg( stv0910_regs.RSTV0910_P2_VAVSRVIT,       0x00 ), /* P2_VAVSRVIT */
    new STReg( stv0910_regs.RSTV0910_P2_VSTATUSVIT,     0xa6 ), /* P2_VSTATUSVIT */
    new STReg( stv0910_regs.RSTV0910_P2_VTHINUSE,       0xd7 ), /* P2_VTHINUSE */
    new STReg( stv0910_regs.RSTV0910_P2_KDIV12,         0x27 ), /* P2_KDIV12 */
    new STReg( stv0910_regs.RSTV0910_P2_KDIV23,         0x32 ), /* P2_KDIV23 */
    new STReg( stv0910_regs.RSTV0910_P2_KDIV34,         0x32 ), /* P2_KDIV34 */
    new STReg( stv0910_regs.RSTV0910_P2_KDIV56,         0x32 ), /* P2_KDIV56 */
    new STReg( stv0910_regs.RSTV0910_P2_KDIV67,         0x32 ), /* P2_KDIV67 */
    new STReg( stv0910_regs.RSTV0910_P2_KDIV78,         0x50 ), /* P2_KDIV78 */
    new STReg( stv0910_regs.RSTV0910_P2_TSPIDFLT1,      0x00 ), /* P2_TSPIDFLT1 */
    new STReg( stv0910_regs.RSTV0910_P2_TSPIDFLT0,      0x00 ), /* P2_TSPIDFLT0 */

    /* DVB2 P2 Registers */


    /* DMD P1 Registers */
    new STReg( stv0910_regs.RSTV0910_P1_IQCONST,        0x00 ), /* P1_IQCONST */
    new STReg( stv0910_regs.RSTV0910_P1_NOSCFG,  0x20 +       0x14 ), /* P1_NOSCFG */
    new STReg( stv0910_regs.RSTV0910_P1_ISYMB,          0x0e ), /* P1_ISYMB         was 0xfe */
    new STReg( stv0910_regs.RSTV0910_P1_QSYMB,          0xf7 ), /* P1_QSYMB         was 0x07 */
    new STReg( stv0910_regs.RSTV0910_P1_AGC1CFG,        0x54 ), /* P1_AGC1CFG */
    new STReg( stv0910_regs.RSTV0910_P1_AGC1CN,         0x99 ), /* P1_AGC1CN */
    new STReg( stv0910_regs.RSTV0910_P1_AGC1REF,        0x58 ), /* P1_AGC1REF */
    new STReg( stv0910_regs.RSTV0910_P1_IDCCOMP,        0x0a ), /* P1_IDCCOMP       was 0x09 */
    new STReg( stv0910_regs.RSTV0910_P1_QDCCOMP,        0x09 ), /* P1_QDCCOMP       was 0xf6 */
    new STReg( stv0910_regs.RSTV0910_P1_POWERI,         0x09 ), /* P1_POWERI */
    new STReg( stv0910_regs.RSTV0910_P1_POWERQ,         0x0a ), /* P1_POWERQ        was 0x09 */
    new STReg( stv0910_regs.RSTV0910_P1_AGC1AMM,        0xfd ), /* P1_AGC1AMM */
///    new STReg( stv0910_regs.RSTV0910_P1_AGC1QUAD,       0x05 ), /* P1_AGC1QUAD */
    new STReg( stv0910_regs.RSTV0910_P1_AGC1QUAD,       0xfd ), /* P1_AGC1QUAD */
    new STReg( stv0910_regs.RSTV0910_P1_AGCIQIN1,       0x00 ), /* P1_AGCIQIN1 */
    new STReg( stv0910_regs.RSTV0910_P1_AGCIQIN0,       0x00 ), /* P1_AGCIQIN0 */
    /* demodulator registers */
    new STReg( stv0910_regs.RSTV0910_P1_DEMOD,          0x00 ), /* P1_DEMOD         auto rolloff, auto spectral inversion, auto DVBs1 rolloff, 35% */
    new STReg( stv0910_regs.RSTV0910_P1_DMDMODCOD,      0x10 ), /* P1_DMDMODCOD     mmodcod auto */
/*  new STReg( stv0910_regs.RSTV0910_P1_DSTATUS,        0x10 ),    P1_DSTATUS       R only */
/*  new STReg( stv0910_regs.RSTV0910_P1_DSTATUS2,       0x80 ),    P1_DSTATUS2      R only */
    new STReg( stv0910_regs.RSTV0910_P1_DMDCFGMD,       0xc9 ), /* P1_DMDCFGMD */
    new STReg( stv0910_regs.RSTV0910_P1_DMDCFG2,        0x3b ), /* P1_DMDCFG2       parallel search (DVBS/S2,  infinite relock tries,  */
    new STReg( stv0910_regs.RSTV0910_P1_DMDISTATE,      0x5c ), /* P1_DMDISTATE */   
    new STReg( stv0910_regs.RSTV0910_P1_DMDT0M,         0x40 ), /* P1_DMDT0M */
/*  new STReg( stv0910_regs.RSTV0910_P1_DMDSTATE,       0x1c ),    P1_DMDSTATE      R only */
/*  new STReg( stv0910_regs.RSTV0910_P1_DMDFLYW,        0x00 ),    P1_DMDFLYW       R only */
/*  new STReg( stv0910_regs.RSTV0910_P1_DSTATUS3,       0x80 ),    P1_DSTATUS3      R only */
    new STReg( stv0910_regs.RSTV0910_P1_DMDCFG3,        0x08 ), /* P1_DMDCFG3       if FIFO is full lose data */
    new STReg( stv0910_regs.RSTV0910_P1_DMDCFG4,        0x04 ), /* P1_DMDCFG4       soft increment of tuner */
    /* correlator registers */
    new STReg( stv0910_regs.RSTV0910_P1_CORRELMANT,     0x78 ), /* P1_CORRELMANT */
    new STReg( stv0910_regs.RSTV0910_P1_CORRELABS,      0x8c ), /* P1_CORRELABS */
    new STReg( stv0910_regs.RSTV0910_P1_CORRELEXP,      0xaa ), /* P1_CORRELEXP */
    new STReg( stv0910_regs.RSTV0910_P1_PLHMODCOD,      0x10 ), /* P1_PLHMODCOD */
    new STReg( stv0910_regs.RSTV0910_P1_DMDREG,         0x01 ), /* P1_DMDREG        eliminates 1 frame in 2 if noise high */
    /* AGC regigesters */
    new STReg( stv0910_regs.RSTV0910_P1_AGCNADJ,        0x00 ), /* P1_AGCNADJ */
    new STReg( stv0910_regs.RSTV0910_P1_AGCKS,          0x00 ), /* P1_AGCKS */
    new STReg( stv0910_regs.RSTV0910_P1_AGCKQ,          0x00 ), /* P1_AGCKQ */
    new STReg( stv0910_regs.RSTV0910_P1_AGCK8,          0x00 ), /* P1_AGCK8 */
    new STReg( stv0910_regs.RSTV0910_P1_AGCK16,         0x00 ), /* P1_AGCK16 */
    new STReg( stv0910_regs.RSTV0910_P1_AGCK32,         0x00 ), /* P1_AGCK32 */
    new STReg( stv0910_regs.RSTV0910_P1_AGC2O,          0x5b ), /* P1_AGC2O */
    new STReg( stv0910_regs.RSTV0910_P1_AGC2REF,        0x38 ), /* P1_AGC2REF */
    new STReg( stv0910_regs.RSTV0910_P1_AGC1ADJ,        0x58 ), /* P1_AGC1ADJ */
    new STReg( stv0910_regs.RSTV0910_P1_AGCRSADJ,       0x38 ), /* P1_AGCRSADJ */
    new STReg( stv0910_regs.RSTV0910_P1_AGCRQADJ,       0x38 ), /* P1_AGCRQADJ */
    new STReg( stv0910_regs.RSTV0910_P1_AGCR8ADJ,       0x38 ), /* P1_AGCR8ADJ */
    new STReg( stv0910_regs.RSTV0910_P1_AGCR1ADJ,       0x38 ), /* P1_AGCR1ADJ */
    new STReg( stv0910_regs.RSTV0910_P1_AGCR2ADJ,       0x38 ), /* P1_AGCR2ADJ */
    new STReg( stv0910_regs.RSTV0910_P1_AGCR3ADJ,       0x47 ), /* P1_AGCR3ADJ */
    new STReg( stv0910_regs.RSTV0910_P1_AGCREFADJ,      0x38 ), /* P1_AGCREFADJ */
    new STReg( stv0910_regs.RSTV0910_P1_AGC2I1,         0x1c ), /* P1_AGC2I1        was 0x0b */
    new STReg( stv0910_regs.RSTV0910_P1_AGC2I0,         0x74 ), /* P1_AGC2I0        was 0x61*/
    /* carrier loop registers */
    new STReg( stv0910_regs.RSTV0910_P1_CARCFG,         0x46 ), /* P1_CARCFG        de_rotator is active, algorithm=citroes 2 */
    new STReg( stv0910_regs.RSTV0910_P1_ACLC,           0x2b ), /* P1_ACLC          alpha=2:0xb mostly fastest DVBS use */
    new STReg( stv0910_regs.RSTV0910_P1_BCLC,           0x1a ), /* P1_BCLC          beta=1:0xa DVBS use */
    new STReg( stv0910_regs.RSTV0910_P1_ACLCS2,         0x00 ), /* P1_ACLCS2    */
    new STReg( stv0910_regs.RSTV0910_P1_BCLCS2,         0x00 ), /* P1_BCLCS2    */
    new STReg( stv0910_regs.RSTV0910_P1_CARFREQ,        0x79 ), /* P1_CARFREQ       course loop coeff=0x7 (mid) DVBS1 beta freq coeff=0x9 mid */
    new STReg( stv0910_regs.RSTV0910_P1_CARHDR,         0x1c ), /* P1_CARHDR        DVBS2 freq_hdr */ 
    new STReg( stv0910_regs.RSTV0910_P1_LDT,            0xd0 ), /* P1_LDT           +ve threshold for lock detect =0xd0 (reset value)*/
    new STReg( stv0910_regs.RSTV0910_P1_LDT2,           0xb8 ), /* P1_LDT2          -ve threshold =0xb8 (reset) */
    new STReg( stv0910_regs.RSTV0910_P1_CFRICFG,        0xf9 ), /* P1_CFRICFG       was 0xf8 CFRINIT +ve increments mode */
    new STReg( stv0910_regs.RSTV0910_P1_CFRUP1,         0x0e ), /* P1_CFRUP1 */
    new STReg( stv0910_regs.RSTV0910_P1_CFRUP0,         0x69 ), /* P1_CFRUP0 */
    new STReg( stv0910_regs.RSTV0910_P1_CFRIBASE1,      0x01 ), /* P1_CFRIBASE1     was 0xff */
    new STReg( stv0910_regs.RSTV0910_P1_CFRIBASE0,      0xf5 ), /* P1_CFRIBASE0     was 0x7a */
    new STReg( stv0910_regs.RSTV0910_P1_CFRLOW1,        0xf1 ), /* P1_CFRLOW1       lower limit if not in auto MSByte */
    new STReg( stv0910_regs.RSTV0910_P1_CFRLOW0,        0x97 ), /* P1_CFRLOW0                                  LSByte */
    new STReg( stv0910_regs.RSTV0910_P1_CFRINIT1,       0x01 ), /* P1_CFRINIT1      was 0xff inital carrier offset                   MSByte */
    new STReg( stv0910_regs.RSTV0910_P1_CFRINIT0,       0xf5 ), /* P1_CFRINIT0      was 0x7a  cfr_init (MHz)=ckadc * CFR_INIT /2^16  LSByte */
    new STReg( stv0910_regs.RSTV0910_P1_CFRINC1,        0x03 ), /* P1_CFRINC1       step size (MHz)=MCLK*CFR_INC/2^13 [13:8) */
    new STReg( stv0910_regs.RSTV0910_P1_CFRINC0,        0x8e ), /* P1_CFRINC0       was 0x10            step size                         [7:0] 
                                                                 note someting off about spec here .. last 2 bits */
    new STReg( stv0910_regs.RSTV0910_P1_CFR2,           0x01 ), /* P1_CFR2          was 0xff carrier offset (in MHz) - mclk * freq / 2^24 [23:16] */
    new STReg( stv0910_regs.RSTV0910_P1_CFR1,           0xf5 ), /* P1_CFR1          was 0x7a                                              [15:8]  */
    new STReg( stv0910_regs.RSTV0910_P1_CFR0,           0x00 ), /* P1_CFR0                                                                [7:0]   */
    new STReg( stv0910_regs.RSTV0910_P1_LDI,            0xb6 ), /* P1_LDI           carrier lock indicator accumulator. used with LDT/LDT2 to 
                                                                 generate DTSATUS.CAR_LOCK */
    /* timing loop registers */
    new STReg( stv0910_regs.RSTV0910_P1_TMGCFG,         0xd3 ), /* P1_TMGCFG        lock indicator fastest, DVBS2 usr SR calculated on 2ns PLHeader
                                                                 compensate for SR min rate min = 1/2048 MCLK (0.066Msymbols@135MHz) */
    new STReg( stv0910_regs.RSTV0910_P1_RTC,            0x68 ), /* P1_RTC           DVBS1 alpha=6 (under mid), beta=8 (mid) */
    new STReg( stv0910_regs.RSTV0910_P1_RTCS2,          0x68 ), /* P1_RTCS2         DVBS2 alpha=6 (under mid), beta=8 (mid) */
    new STReg( stv0910_regs.RSTV0910_P1_TMGTHRISE,      0x1e ), /* P1_TMGTHRISE     +ve edge of timing lock indicator used in QUALITY in DSTATUS */
    new STReg( stv0910_regs.RSTV0910_P1_TMGTHFALL,      0x08 ), /* P1_TMGTHFALL     -ve edge of timing lock indicator used for QUALITY in DSTATUS */
    new STReg( stv0910_regs.RSTV0910_P1_SFRUPRATIO,     0x20 ), /* P1_SFRUPRATIO    was 0x20 updated to 0x01
                                                                 ratio=SFR(INIT)*(1+RATIO/256)(course mode) (/65536 in fine mode)
                                                                 default ratio is 0.125 of SR  */
    new STReg( stv0910_regs.RSTV0910_P1_SFRLOWRATIO,    0xd0 ), /* P1_SFRLOWRATIO   */
    new STReg( stv0910_regs.RSTV0910_P1_KTTMG,          0xa0 ), /* P1_KTTMG         NOT IN DATASHEET */
    new STReg( stv0910_regs.RSTV0910_P1_KREFTMG,        0x80 ), /* P1_KREFTMG       refn level for SR course. 0x80=mid level */ 
    new STReg( stv0910_regs.RSTV0910_P1_SFRSTEP,        0x88 ), /* P1_SFRSTEP       scanstep=mid, centerstep=mid */
    new STReg( stv0910_regs.RSTV0910_P1_TMGCFG2,        0x80 ), /* P1_TMGCFG2       */
/*  new STReg( stv0910_regs.RSTV0910_P1_KREFTMG2,       0x80 ),    P1_KREFTMG2      R only */
    new STReg( stv0910_regs.RSTV0910_P1_TMGCFG3,        0x06 ), /* P1_TMGCFG3       */
    new STReg( stv0910_regs.RSTV0910_P1_SFRINIT1,       0x38 ), /* P1_SFRINIT1      */
    new STReg( stv0910_regs.RSTV0910_P1_SFRINIT0,       0xe3 ), /* P1_SFRINIT0      */
    new STReg( stv0910_regs.RSTV0910_P1_SFRUP1,         0x3f ), /* P1_SFRUP1        was 0x19 
                                                                 when in manual mode, upper symbol rate to try MSByte */
    new STReg( stv0910_regs.RSTV0910_P1_SFRUP0,         0xff ), /* P1_SFRUP0        was 0x99                                               LSByte */
    new STReg( stv0910_regs.RSTV0910_P1_SFRLOW1,        0x2e ), /* P1_SFRLOW1       was 0x12 in manual mode, lower symbol rate to try      MSByte */
    new STReg( stv0910_regs.RSTV0910_P1_SFRLOW0,        0x39 ), /* P1_SFRLOW0       was 0x7d                                               LSByte */
    new STReg( stv0910_regs.RSTV0910_P1_SFR3,           0x38 ), /* P1_SFR3          current symbol rate. SR (MHz)=ckadc (135MHz) * SFR/2^32 MSByte */
    new STReg( stv0910_regs.RSTV0910_P1_SFR2,           0xe3 ), /* P1_SFR2                                                               upper mid byte */
    new STReg( stv0910_regs.RSTV0910_P1_SFR1,           0x00 ), /* P1_SFR1                                                               lower mid byte */
    new STReg( stv0910_regs.RSTV0910_P1_SFR0,           0x00 ), /* P1_SFR0                                                               LSByte */
    new STReg( stv0910_regs.RSTV0910_P1_TMGREG2,        0x00 ), /* P1_TMGREG2       timing recovery accumulator, SR offset MSByte */
    new STReg( stv0910_regs.RSTV0910_P1_TMGREG1,        0x00 ), /* P1_TMGREG1                                              MIDByte */
    new STReg( stv0910_regs.RSTV0910_P1_TMGREG0,        0x00 ), /* P1_TMGREG0                                              LSByte */
/*  new STReg( stv0910_regs.RSTV0910_P1_TMGLOCK1,       0xe4 ),    P1_TMGLOCK1      raw lock info. used to find tmg_lock Quality in DMSTATUS MSB */
/*  new STReg( stv0910_regs.RSTV0910_P1_TMGLOCK0,       0x00 ),    P1_TMGLOCK0                                                               LSB */
/*  new STReg( stv0910_regs.RSTV0910_P1_TMGOBS,         0x10 ),    P1_TMGOBS        timing loop observation: roll off status */
    /* equaliser registers */
    new STReg( stv0910_regs.RSTV0910_P1_EQUALCFG,       0x41 ), /* P1_EQUALCFG */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAI1,         0x00 ), /* P1_EQUAI1 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAQ1,         0x00 ), /* P1_EQUAQ1 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAI2,         0x00 ), /* P1_EQUAI2 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAQ2,         0x00 ), /* P1_EQUAQ2 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAI3,         0x00 ), /* P1_EQUAI3 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAQ3,         0x00 ), /* P1_EQUAQ3 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAI4,         0x00 ), /* P1_EQUAI4 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAQ4,         0x00 ), /* P1_EQUAQ4 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAI5,         0x00 ), /* P1_EQUAI5 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAQ5,         0x00 ), /* P1_EQUAQ5 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAI6,         0x00 ), /* P1_EQUAI6 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAQ6,         0x00 ), /* P1_EQUAQ6 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAI7,         0x00 ), /* P1_EQUAI7 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAQ7,         0x00 ), /* P1_EQUAQ7 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAI8,         0x00 ), /* P1_EQUAI8 */
    new STReg( stv0910_regs.RSTV0910_P1_EQUAQ8,         0x00 ), /* P1_EQUAQ8 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSDATAT1,     0xff ), /* P1_NNOSDATAT1 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSDATAT0,     0xff ), /* P1_NNOSDATAT0 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSDATA1,      0xff ), /* P1_NNOSDATA1 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSDATA0,      0xff ), /* P1_NNOSDATA0 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSPLHT1,      0xff ), /* P1_NNOSPLHT1 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSPLHT0,      0xff ), /* P1_NNOSPLHT0 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSPLH1,       0xff ), /* P1_NNOSPLH1 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSPLH0,       0xff ), /* P1_NNOSPLH0 */
    new STReg( stv0910_regs.RSTV0910_P1_NOSDATAT1,      0xff ), /* P1_NOSDATAT1 */
    new STReg( stv0910_regs.RSTV0910_P1_NOSDATAT0,      0xff ), /* P1_NOSDATAT0 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSFRAME1,     0xff ), /* P1_NNOSFRAME1 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSFRAME0,     0xff ), /* P1_NNOSFRAME0 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSRAD1,       0x00 ), /* P1_NNOSRAD1 */
    new STReg( stv0910_regs.RSTV0910_P1_NNOSRAD0,       0x00 ), /* P1_NNOSRAD0 */
    new STReg( stv0910_regs.RSTV0910_P1_NOSCFGF1,       0x00 ), /* P1_NOSCFGF1 */
    new STReg( stv0910_regs.RSTV0910_P1_NOSCFGF2,       0x00 ), /* P1_NOSCFGF2 */
    new STReg( stv0910_regs.RSTV0910_P1_CAR2CFG,        0x06 ), /* P1_CAR2CFG */
    new STReg( stv0910_regs.RSTV0910_P1_CFR2CFR1,       0xe5 ), /* P1_CFR2CFR1 */
    new STReg( stv0910_regs.RSTV0910_P1_CAR3CFG,        0x02 ), /* P1_CAR3CFG */
    new STReg( stv0910_regs.RSTV0910_P1_CFR22,          0x00 ), /* P1_CFR22 */
    new STReg( stv0910_regs.RSTV0910_P1_CFR21,          0x00 ), /* P1_CFR21 */
    new STReg( stv0910_regs.RSTV0910_P1_CFR20,          0x00 ), /* P1_CFR20 */
    new STReg( stv0910_regs.RSTV0910_P1_ACLC2S2Q,       0x0b ), /* P1_ACLC2S2Q */
    new STReg( stv0910_regs.RSTV0910_P1_ACLC2S28,       0x0a ), /* P1_ACLC2S28 */
    new STReg( stv0910_regs.RSTV0910_P1_ACLC2S216A,     0x49 ), /* P1_ACLC2S216A */
    new STReg( stv0910_regs.RSTV0910_P1_ACLC2S232A,     0x48 ), /* P1_ACLC2S232A */
    new STReg( stv0910_regs.RSTV0910_P1_BCLC2S2Q,       0x84 ), /* P1_BCLC2S2Q */
    new STReg( stv0910_regs.RSTV0910_P1_BCLC2S28,       0x84 ), /* P1_BCLC2S28 */
    new STReg( stv0910_regs.RSTV0910_P1_BCLC2S216A,     0x84 ), /* P1_BCLC2S216A */
    new STReg( stv0910_regs.RSTV0910_P1_BCLC2S232A,     0x84 ), /* P1_BCLC2S232A */
    new STReg( stv0910_regs.RSTV0910_P1_PLROOT2,        0x00 ), /* P1_PLROOT2 */
    new STReg( stv0910_regs.RSTV0910_P1_PLROOT1,        0x00 ), /* P1_PLROOT1 */
    new STReg( stv0910_regs.RSTV0910_P1_PLROOT0,        0x01 ), /* P1_PLROOT0 */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST0,     0xff ), /* P1_MODCODLST0 */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST1,     0xfc ), /* P1_MODCODLST1    was 0xcf */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST2,     0x00 ), /* P1_MODCODLST2    was 0xff */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST3,     0x00 ), /* P1_MODCODLST3    was 0xff */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST4,     0x00 ), /* P1_MODCODLST4 */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST5,     0x00 ), /* P1_MODCODLST5 */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST6,     0x00 ), /* P1_MODCODLST6 */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST7,     0xc0 ), /* P1_MODCODLST7 */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST8,     0x00 ), /* P1_MODCODLST8 */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLST9,     0x00 ), /* P1_MODCODLST9 */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLSTA,     0xc0 ), /* P1_MODCODLSTA */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLSTB,     0x00 ), /* P1_MODCODLSTB */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLSTC,     0x00 ), /* P1_MODCODLSTC */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLSTD,     0x00 ), /* P1_MODCODLSTD */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLSTE,     0x00 ), /* P1_MODCODLSTE */
    new STReg( stv0910_regs.RSTV0910_P1_MODCODLSTF,     0x0f ), /* P1_MODCODLSTF */
    new STReg( stv0910_regs.RSTV0910_P1_GAUSSR0,        0x98 ), /* P1_GAUSSR0 */
    new STReg( stv0910_regs.RSTV0910_P1_CCIR0,          0x30 ), /* P1_CCIR0 */
    new STReg( stv0910_regs.RSTV0910_P1_CCIQUANT,       0xac ), /* P1_CCIQUANT */
    new STReg( stv0910_regs.RSTV0910_P1_CCITHRES,       0x50 ), /* P1_CCITHRES */
    new STReg( stv0910_regs.RSTV0910_P1_CCIACC,         0x00 ), /* P1_CCIACC */
    new STReg( stv0910_regs.RSTV0910_P1_DSTATUS4,       0x64 ), /* P1_DSTATUS4 */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESCFG,      0x29 ), /* P1_DMDRESCFG */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESADR,      0x00 ), /* P1_DMDRESADR */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESDATA7,    0x00 ), /* P1_DMDRESDATA7 */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESDATA6,    0x00 ), /* P1_DMDRESDATA6 */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESDATA5,    0x00 ), /* P1_DMDRESDATA5 */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESDATA4,    0x00 ), /* P1_DMDRESDATA4 */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESDATA3,    0x00 ), /* P1_DMDRESDATA3 */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESDATA2,    0x00 ), /* P1_DMDRESDATA2 */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESDATA1,    0x00 ), /* P1_DMDRESDATA1 */
    new STReg( stv0910_regs.RSTV0910_P1_DMDRESDATA0,    0x00 ), /* P1_DMDRESDATA0 */
    new STReg( stv0910_regs.RSTV0910_P1_FFEI1,          0x00 ), /* P1_FFEI1 */
    new STReg( stv0910_regs.RSTV0910_P1_FFEQ1,          0x00 ), /* P1_FFEQ1 */
    new STReg( stv0910_regs.RSTV0910_P1_FFEI2,          0x00 ), /* P1_FFEI2 */
    new STReg( stv0910_regs.RSTV0910_P1_FFEQ2,          0x00 ), /* P1_FFEQ2 */
    new STReg( stv0910_regs.RSTV0910_P1_FFEI3,          0x00 ), /* P1_FFEI3 */
    new STReg( stv0910_regs.RSTV0910_P1_FFEQ3,          0x00 ), /* P1_FFEQ3 */
    new STReg( stv0910_regs.RSTV0910_P1_FFEI4,          0x00 ), /* P1_FFEI4 */
    new STReg( stv0910_regs.RSTV0910_P1_FFEQ4,          0x00 ), /* P1_FFEQ4 */
    new STReg( stv0910_regs.RSTV0910_P1_FFECFG,         0x71 ), /* P1_FFECFG */
    /* TUN Register */
    new STReg( stv0910_regs.RSTV0910_P1_TNRCFG2,        0x02 ), /* P1_TNRCFG2 */
    new STReg( stv0910_regs.RSTV0910_P1_SMAPCOEF7,      0x00 ), /* P1_SMAPCOEF7 */
    new STReg( stv0910_regs.RSTV0910_P1_SMAPCOEF6,      0x00 ), /* P1_SMAPCOEF6 */
    new STReg( stv0910_regs.RSTV0910_P1_SMAPCOEF5,      0x00 ), /* P1_SMAPCOEF5 */
    new STReg( stv0910_regs.RSTV0910_P1_SMAPCOEF4,      0x00 ), /* P1_SMAPCOEF4 */
    new STReg( stv0910_regs.RSTV0910_P1_SMAPCOEF3,      0x00 ), /* P1_SMAPCOEF3 */
    new STReg( stv0910_regs.RSTV0910_P1_SMAPCOEF2,      0x00 ), /* P1_SMAPCOEF2 */
    new STReg( stv0910_regs.RSTV0910_P1_SMAPCOEF1,      0x00 ), /* P1_SMAPCOEF1 */
    new STReg( stv0910_regs.RSTV0910_P1_SMAPCOEF0,      0x71 ), /* P1_SMAPCOEF0 */
    new STReg( stv0910_regs.RSTV0910_P1_NOSTHRES1,      0x60 ), /* P1_NOSTHRES1 */
    new STReg( stv0910_regs.RSTV0910_P1_NOSTHRES2,      0x69 ), /* P1_NOSTHRES2 */
    new STReg( stv0910_regs.RSTV0910_P1_NOSDIFF1,       0x80 ), /* P1_NOSDIFF1 */
    new STReg( stv0910_regs.RSTV0910_P1_RAINFADE,       0x35 ), /* P1_RAINFADE */
    new STReg( stv0910_regs.RSTV0910_P1_NOSRAMCFG,      0x28 ), /* P1_NOSRAMCFG */
    new STReg( stv0910_regs.RSTV0910_P1_NOSRAMPOS,      0x26 ), /* P1_NOSRAMPOS */
    new STReg( stv0910_regs.RSTV0910_P1_NOSRAMVAL,      0x86 ), /* P1_NOSRAMVAL */
    new STReg( stv0910_regs.RSTV0910_P1_DMDPLHSTAT,     0x00 ), /* P1_DMDPLHSTAT */
    new STReg( stv0910_regs.RSTV0910_P1_LOCKTIME3,      0xc0 ), /* P1_LOCKTIME3 */
    new STReg( stv0910_regs.RSTV0910_P1_LOCKTIME2,      0xd2 ), /* P1_LOCKTIME2     was 0xd1 */
    new STReg( stv0910_regs.RSTV0910_P1_LOCKTIME1,      0x3b ), /* P1_LOCKTIME1     was 0x05 */
    new STReg( stv0910_regs.RSTV0910_P1_LOCKTIME0,      0x46 ), /* P1_LOCKTIME0     was 0xc8 */
    /* DVB1 P1 Registers */
    new STReg( stv0910_regs.RSTV0910_P1_VITSCALE,       0x80 ), /* P1_VITSCALE */
    new STReg( stv0910_regs.RSTV0910_P1_FECM,           0x00 ), /* P1_FECM */
    new STReg( stv0910_regs.RSTV0910_P1_VTH12,          0xd7 ), /* P1_VTH12 */
    new STReg( stv0910_regs.RSTV0910_P1_VTH23,          0x85 ), /* P1_VTH23 */
    new STReg( stv0910_regs.RSTV0910_P1_VTH34,          0x58 ), /* P1_VTH34 */
    new STReg( stv0910_regs.RSTV0910_P1_VTH56,          0x3a ), /* P1_VTH56 */
    new STReg( stv0910_regs.RSTV0910_P1_VTH67,          0x34 ), /* P1_VTH67 */
    new STReg( stv0910_regs.RSTV0910_P1_VTH78,          0x28 ), /* P1_VTH78 */
    new STReg( stv0910_regs.RSTV0910_P1_VITCURPUN,      0x0d ), /* P1_VITCURPUN */
    new STReg( stv0910_regs.RSTV0910_P1_VERROR,         0xff ), /* P1_VERROR */
    new STReg( stv0910_regs.RSTV0910_P1_PRVIT,          0x2f ), /* P1_PRVIT */
    new STReg( stv0910_regs.RSTV0910_P1_VAVSRVIT,       0x00 ), /* P1_VAVSRVIT */
    new STReg( stv0910_regs.RSTV0910_P1_VSTATUSVIT,     0xa6 ), /* P1_VSTATUSVIT    was 0xa4 */
    new STReg( stv0910_regs.RSTV0910_P1_VTHINUSE,       0xd7 ), /* P1_VTHINUSE */
    new STReg( stv0910_regs.RSTV0910_P1_KDIV12,         0x27 ), /* P1_KDIV12 */
    new STReg( stv0910_regs.RSTV0910_P1_KDIV23,         0x32 ), /* P1_KDIV23 */
    new STReg( stv0910_regs.RSTV0910_P1_KDIV34,         0x32 ), /* P1_KDIV34 */
    new STReg( stv0910_regs.RSTV0910_P1_KDIV56,         0x32 ), /* P1_KDIV56 */
    new STReg( stv0910_regs.RSTV0910_P1_KDIV67,         0x32 ), /* P1_KDIV67 */
    new STReg( stv0910_regs.RSTV0910_P1_KDIV78,         0x50 ), /* P1_KDIV78 */
    new STReg( stv0910_regs.RSTV0910_P1_TSPIDFLT1,      0x00 ), /* P1_TSPIDFLT1 */
    new STReg( stv0910_regs.RSTV0910_P1_TSPIDFLT0,      0x00 ), /* P1_TSPIDFLT0 */

    /* DVB2 P1 Registers */
    new STReg( stv0910_regs.RSTV0910_TSGENERAL,         0x00 ), // TSGENERAL        enable output of second line in parallel line
///                                                                 override tsfifo_permparal and defineline1->TS3, line2->TS2,RCline->TS1
///                                                                 tsfifo_perparal defines line1-> TS3, line2->TS2, RC LIne->TS1 
    /* DISEQC P1 Registers */
    new STReg( stv0910_regs.RSTV0910_P1_DISIRQCFG,      0x00 ), /* P1_DISIRQCFG     */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISIRQSTAT,     0x00 ),    P1_DISIRQSTAT    R only */
    new STReg( stv0910_regs.RSTV0910_P1_DISTXCFG,       0x02 ), /* P1_DISTXCFG      */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISTXSTATUS,    0x20 ),    P1_DISTXSTATUS   R only */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISTXBYTES,     0x00 ),    P1_DISTXBYTES    R only */
    new STReg( stv0910_regs.RSTV0910_P1_DISTXFIFO,      0x00 ), /* P1_DISTXFIFO     */
    new STReg( stv0910_regs.RSTV0910_P1_DISTXF22,       0xc0 ), /* P1_DISTXF22      */
    new STReg( stv0910_regs.RSTV0910_P1_DISTIMEOCFG,    0x02 ), /* P1_DISTIMEOCFG   */
    new STReg( stv0910_regs.RSTV0910_P1_DISTIMEOUT,     0x8c ), /* P1_DISTIMEOUT    */
    new STReg( stv0910_regs.RSTV0910_P1_DISRXCFG,       0x34 ), /* P1_DISRXCFG      */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISRXSTAT1,     0x04 ),    P1_DISRXSTAT1    R only */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISRXSTAT0,     0x00 ),    P1_DISRXSTAT0    R only */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISRXBYTES,     0x00 ),    P1_DISRXBYTES    R only */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISRXPARITY1,   0x00 ),    P1_DISRXPARITY1  R only */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISRXPARITY0,   0x00 ),    P1_DISRXPARITY0  R only */
    new STReg( stv0910_regs.RSTV0910_P1_DISRXFIFO,      0x00 ), /* P1_DISRXFIFO     */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISRXDC1,       0x03 ),    P1_DISRXDC1      R only */
/*  new STReg( stv0910_regs.RSTV0910_P1_DISRXDC0,       0xf1 ),    P1_DISRXDC0      R only was 0xf6 */
    new STReg( stv0910_regs.RSTV0910_P1_DISRXF221,      0x01 ), /* P1_DISRXF221     */
    new STReg( stv0910_regs.RSTV0910_P1_DISRXF220,      0x2b ), /* P1_DISRXF220     */
    new STReg( stv0910_regs.RSTV0910_P1_DISRXF100,      0xa9 ), /* P1_DISRXF100     */
    new STReg( stv0910_regs.RSTV0910_P1_DISRXSHORT22K,  0x0f ), /* P1_DISRXSHORT22K */

    /* P1 aux clock control registers */
    new STReg( stv0910_regs.RSTV0910_P1_ACRPRESC,       0x01 ), /* P1_ACRPRESC      auxiliary clock - not used */
    new STReg( stv0910_regs.RSTV0910_P1_ACRDIV,         0x14 ), /* P1_ACRDIV        auxiliary clock - not used */

    /* DISEQC P2 Registers */
    new STReg( stv0910_regs.RSTV0910_P2_DISIRQCFG,      0x00 ), /* P2_DISIRQCFG     as above for P2 */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISIRQSTAT,     0x00 ),    P2_DISIRQSTAT    */
    new STReg( stv0910_regs.RSTV0910_P2_DISTXCFG,       0x02 ), /* P2_DISTXCFG      */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISTXSTATUS,    0x20 ),    P2_DISTXSTATUS   */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISTXBYTES,     0x00 ),    P2_DISTXBYTES    */
    new STReg( stv0910_regs.RSTV0910_P2_DISTXFIFO,      0x00 ), /* P2_DISTXFIFO     */
    new STReg( stv0910_regs.RSTV0910_P2_DISTXF22,       0xc0 ), /* P2_DISTXF22      */
    new STReg( stv0910_regs.RSTV0910_P2_DISTIMEOCFG,    0x02 ), /* P2_DISTIMEOCFG   */
    new STReg( stv0910_regs.RSTV0910_P2_DISTIMEOUT,     0x8c ), /* P2_DISTIMEOUT    */
    new STReg( stv0910_regs.RSTV0910_P2_DISRXCFG,       0x34 ), /* P2_DISRXCFG      */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISRXSTAT1,     0x04 ),    P2_DISRXSTAT1    */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISRXSTAT0,     0x00 ),    P2_DISRXSTAT0    */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISRXBYTES,     0x00 ),    P2_DISRXBYTES    */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISRXPARITY1,   0x00 ),    P2_DISRXPARITY1  */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISRXPARITY0,   0x00 ),    P2_DISRXPARITY0  */
    new STReg( stv0910_regs.RSTV0910_P2_DISRXFIFO,      0x00 ), /* P2_DISRXFIFO     */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISRXDC1,       0x03 ),    P2_DISRXDC1      */
/*  new STReg( stv0910_regs.RSTV0910_P2_DISRXDC0,       0xf1 ),    P2_DISRXDC0      */
    new STReg( stv0910_regs.RSTV0910_P2_DISRXF221,      0x01 ), /* P2_DISRXF221     */
    new STReg( stv0910_regs.RSTV0910_P2_DISRXF220,      0x2b ), /* P2_DISRXF220     */
    new STReg( stv0910_regs.RSTV0910_P2_DISRXF100,      0xa9 ), /* P2_DISRXF100     */
    new STReg( stv0910_regs.RSTV0910_P2_DISRXSHORT22K,  0x0f ), /* P2_DISRXSHORT22K */

    /* P2 aux clock control registers */
    new STReg( stv0910_regs.RSTV0910_P2_ACRPRESC,       0x01 ), /* P2_ACRPRESC      auxiliary clock - not used */ 
    new STReg( stv0910_regs.RSTV0910_P2_ACRDIV,         0x14 ), /* P2_ACRDIV        auxiliary clock - not used */

    /* P1 NBITER registers */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF1,     0x1c ), /* P1_NBITER_NF1 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF2,     0x19 ), /* P1_NBITER_NF2 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF3,     0x17 ), /* P1_NBITER_NF3 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF4,     0x18 ), /* P1_NBITER_NF4 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF5,     0x13 ), /* P1_NBITER_NF5 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF6,     0x19 ), /* P1_NBITER_NF6 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF7,     0x18 ), /* P1_NBITER_NF7 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF8,     0x17 ), /* P1_NBITER_NF8 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF9,     0x16 ), /* P1_NBITER_NF9 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF10,    0x1c ), /* P1_NBITER_NF10 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF11,    0x1c ), /* P1_NBITER_NF11 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF12,    0x13 ), /* P1_NBITER_NF12 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF13,    0x19 ), /* P1_NBITER_NF13 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF14,    0x18 ), /* P1_NBITER_NF14 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF15,    0x16 ), /* P1_NBITER_NF15 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF16,    0x1c ), /* P1_NBITER_NF16 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF17,    0x1c ), /* P1_NBITER_NF17 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF18,    0x19 ), /* P1_NBITER_NF18 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF19,    0x18 ), /* P1_NBITER_NF19 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF20,    0x17 ), /* P1_NBITER_NF20 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF21,    0x16 ), /* P1_NBITER_NF21 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF22,    0x1c ), /* P1_NBITER_NF22 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF23,    0x1c ), /* P1_NBITER_NF23 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF24,    0x18 ), /* P1_NBITER_NF24 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF25,    0x17 ), /* P1_NBITER_NF25 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF26,    0x16 ), /* P1_NBITER_NF26 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF27,    0x1c ), /* P1_NBITER_NF27 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_NF28,    0x1c ), /* P1_NBITER_NF28 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF1,     0x1b ), /* P1_NBITER_SF1 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF2,     0x19 ), /* P1_NBITER_SF2 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF3,     0x17 ), /* P1_NBITER_SF3 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF4,     0x1a ), /* P1_NBITER_SF4 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF5,     0x13 ), /* P1_NBITER_SF5 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF6,     0x19 ), /* P1_NBITER_SF6 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF7,     0x1b ), /* P1_NBITER_SF7 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF8,     0x1d ), /* P1_NBITER_SF8 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF9,     0x1b ), /* P1_NBITER_SF9 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF10,    0x1c ), /* P1_NBITER_SF10 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF12,    0x13 ), /* P1_NBITER_SF12 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF13,    0x19 ), /* P1_NBITER_SF13 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF14,    0x1b ), /* P1_NBITER_SF14 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF15,    0x1b ), /* P1_NBITER_SF15 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF16,    0x1c ), /* P1_NBITER_SF16 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF18,    0x19 ), /* P1_NBITER_SF18 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF19,    0x1b ), /* P1_NBITER_SF19 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF20,    0x1d ), /* P1_NBITER_SF20 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF21,    0x1b ), /* P1_NBITER_SF21 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF22,    0x1c ), /* P1_NBITER_SF22 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF24,    0x1b ), /* P1_NBITER_SF24 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF25,    0x1d ), /* P1_NBITER_SF25 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF26,    0x1b ), /* P1_NBITER_SF26 */
    new STReg( stv0910_regs.RSTV0910_P1_NBITER_SF27,    0x1c ), /* P1_NBITER_SF27 */

    new STReg( stv0910_regs.RSTV0910_SELSATUR6,         0x05 ), /* SELSATUR6        */
    new STReg( stv0910_regs.RSTV0910_SELSATUR5,         0x5b ), /* SELSATUR5        */
    new STReg( stv0910_regs.RSTV0910_SELSATUR4,         0x96 ), /* SELSATUR4        */
    new STReg( stv0910_regs.RSTV0910_SELSATUR3,         0x00 ), /* SELSATUR3        */
    new STReg( stv0910_regs.RSTV0910_SELSATUR2,         0x00 ), /* SELSATUR2        */
    new STReg( stv0910_regs.RSTV0910_SELSATUR1,         0x00 ), /* SELSATUR1        */
    new STReg( stv0910_regs.RSTV0910_SELSATUR0,         0x00 ), /* SELSATUR0        */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF1,       0x20 ), /* GAINLLR_NF1      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF2,       0x20 ), /* GAINLLR_NF2      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF3,       0x20 ), /* GAINLLR_NF3      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF4,       0x20 ), /* GAINLLR_NF4      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF5,       0x20 ), /* GAINLLR_NF5      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF6,       0x20 ), /* GAINLLR_NF6      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF7,       0x20 ), /* GAINLLR_NF7      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF8,       0x20 ), /* GAINLLR_NF8      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF9,       0x20 ), /* GAINLLR_NF9      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF10,      0x20 ), /* GAINLLR_NF10     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF11,      0x20 ), /* GAINLLR_NF11     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF12,      0x20 ), /* GAINLLR_NF12     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF13,      0x20 ), /* GAINLLR_NF13     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF14,      0x20 ), /* GAINLLR_NF14     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF15,      0x20 ), /* GAINLLR_NF15     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF16,      0x20 ), /* GAINLLR_NF16     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF17,      0x20 ), /* GAINLLR_NF17     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF18,      0x22 ), /* GAINLLR_NF18     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF19,      0x22 ), /* GAINLLR_NF19     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF20,      0x24 ), /* GAINLLR_NF20     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF21,      0x24 ), /* GAINLLR_NF21     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF22,      0x25 ), /* GAINLLR_NF22     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF23,      0x26 ), /* GAINLLR_NF23     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF24,      0x20 ), /* GAINLLR_NF24     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF25,      0x20 ), /* GAINLLR_NF25     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF26,      0x20 ), /* GAINLLR_NF26     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF27,      0x20 ), /* GAINLLR_NF27     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_NF28,      0x20 ), /* GAINLLR_NF28     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF1,       0x20 ), /* GAINLLR_SF1      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF2,       0x20 ), /* GAINLLR_SF2      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF3,       0x20 ), /* GAINLLR_SF3      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF4,       0x20 ), /* GAINLLR_SF4      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF5,       0x20 ), /* GAINLLR_SF5      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF6,       0x20 ), /* GAINLLR_SF6      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF7,       0x20 ), /* GAINLLR_SF7      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF8,       0x20 ), /* GAINLLR_SF8      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF9,       0x20 ), /* GAINLLR_SF9      */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF10,      0x20 ), /* GAINLLR_SF10     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF12,      0x20 ), /* GAINLLR_SF12     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF13,      0x20 ), /* GAINLLR_SF13     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF14,      0x20 ), /* GAINLLR_SF14     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF15,      0x20 ), /* GAINLLR_SF15     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF16,      0x20 ), /* GAINLLR_SF16     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF18,      0x22 ), /* GAINLLR_SF18     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF19,      0x22 ), /* GAINLLR_SF19     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF20,      0x24 ), /* GAINLLR_SF20     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF21,      0x24 ), /* GAINLLR_SF21     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF22,      0x25 ), /* GAINLLR_SF22     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF24,      0x20 ), /* GAINLLR_SF24     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF25,      0x20 ), /* GAINLLR_SF25     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF26,      0x20 ), /* GAINLLR_SF26     */
    new STReg( stv0910_regs.RSTV0910_GAINLLR_SF27,      0x20 ), /* GAINLLR_SF27     */
    new STReg( stv0910_regs.RSTV0910_CFGEXT,            0x02 ), /* CFGEXT           */
///
/*  new STReg( stv0910_regs.RSTV0910_LDPCERR1,          0x00 ),    LDPCERR1         R only : error counter MSByte */
/*  new STReg( stv0910_regs.RSTV0910_LDPCERR0,          0x00 ),    LDPCERR0         R only : error counter LSByte */
/*  new STReg( stv0910_regs.RSTV0910_BCHERR,            0x00 ),    BCHERR           R only : error flag and error counter */

    new STReg( stv0910_regs.RSTV0910_P1_MAXEXTRAITER,   0x07 ), /* P1_MAXEXTRAITER */
    new STReg( stv0910_regs.RSTV0910_P2_MAXEXTRAITER,   0x07 ), /* P2_MAXEXTRAITER */
    new STReg( stv0910_regs.RSTV0910_P1_STATUSITER,     0x00 ), /* P1_STATUSITER */
    new STReg( stv0910_regs.RSTV0910_P1_STATUSMAXITER,  0x00 ), /* P1_STATUSMAXITER */
    new STReg( stv0910_regs.RSTV0910_P2_STATUSITER,     0x00 ), /* P2_STATUSITER */
    new STReg( stv0910_regs.RSTV0910_P2_STATUSMAXITER,  0x00 ), /* P2_STATUSMAXITER */

    /* P2 NBITER registers */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF1,     0x1c ), /* P2_NBITER_NF1 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF2,     0x19 ), /* P2_NBITER_NF2 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF3,     0x17 ), /* P2_NBITER_NF3 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF4,     0x18 ), /* P2_NBITER_NF4 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF5,     0x13 ), /* P2_NBITER_NF5 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF6,     0x19 ), /* P2_NBITER_NF6 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF7,     0x18 ), /* P2_NBITER_NF7 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF8,     0x17 ), /* P2_NBITER_NF8 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF9,     0x16 ), /* P2_NBITER_NF9 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF10,    0x1c ), /* P2_NBITER_NF10 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF11,    0x1c ), /* P2_NBITER_NF11 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF12,    0x13 ), /* P2_NBITER_NF12 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF13,    0x19 ), /* P2_NBITER_NF13 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF14,    0x18 ), /* P2_NBITER_NF14 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF15,    0x16 ), /* P2_NBITER_NF15 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF16,    0x1c ), /* P2_NBITER_NF16 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF17,    0x1c ), /* P2_NBITER_NF17 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF18,    0x19 ), /* P2_NBITER_NF18 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF19,    0x18 ), /* P2_NBITER_NF19 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF20,    0x17 ), /* P2_NBITER_NF20 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF21,    0x16 ), /* P2_NBITER_NF21 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF22,    0x1c ), /* P2_NBITER_NF22 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF23,    0x1c ), /* P2_NBITER_NF23 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF24,    0x18 ), /* P2_NBITER_NF24 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF25,    0x17 ), /* P2_NBITER_NF25 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF26,    0x16 ), /* P2_NBITER_NF26 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF27,    0x1c ), /* P2_NBITER_NF27 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_NF28,    0x1c ), /* P2_NBITER_NF28 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF1,     0x1b ), /* P2_NBITER_SF1 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF2,     0x19 ), /* P2_NBITER_SF2 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF3,     0x17 ), /* P2_NBITER_SF3 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF4,     0x1a ), /* P2_NBITER_SF4 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF5,     0x13 ), /* P2_NBITER_SF5 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF6,     0x19 ), /* P2_NBITER_SF6 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF7,     0x1b ), /* P2_NBITER_SF7 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF8,     0x1d ), /* P2_NBITER_SF8 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF9,     0x1b ), /* P2_NBITER_SF9 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF10,    0x1c ), /* P2_NBITER_SF10 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF12,    0x13 ), /* P2_NBITER_SF12 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF13,    0x19 ), /* P2_NBITER_SF13 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF14,    0x1b ), /* P2_NBITER_SF14 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF15,    0x1b ), /* P2_NBITER_SF15 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF16,    0x1c ), /* P2_NBITER_SF16 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF18,    0x19 ), /* P2_NBITER_SF18 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF19,    0x1b ), /* P2_NBITER_SF19 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF20,    0x1d ), /* P2_NBITER_SF20 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF21,    0x1b ), /* P2_NBITER_SF21 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF22,    0x1c ), /* P2_NBITER_SF22 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF24,    0x1b ), /* P2_NBITER_SF24 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF25,    0x1d ), /* P2_NBITER_SF25 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF26,    0x1b ), /* P2_NBITER_SF26 */
    new STReg( stv0910_regs.RSTV0910_P2_NBITER_SF27,    0x1c ), /* P2_NBITER_SF27 */


///    new STReg( stv0910_regs.RSTV0910_P1_TSINSDELH,      0x01 ), 		// send CRC at end of packet for DVB-S2
///    new STReg( stv0910_regs.RSTV0910_P2_TSINSDELH,      0x01 ), 		// send CRC at end of packet for DVB-S2   
    new STReg( stv0910_regs.RSTV0910_P1_TSCFGH,         0x80 ), 		// serial output, clock off when no data   
    new STReg( stv0910_regs.RSTV0910_P2_TSCFGH,         0x80 ),
    new STReg( stv0910_regs.RSTV0910_GENCFG,            0x15), 	// dual TS mode -  CROSSOVER bit 1 appears to do nothing
    new STReg( stv0910_regs.RSTV0910_OUTCFG2,           0x00 ), 		// invert VALID and CLOCK 
///    new STReg( stv0910_regs.RSTV0910_P1_TSDIVN,         0x83 ), 		// output clock adapts to data rate
///    new STReg( stv0910_regs.RSTV0910_P2_TSDIVN,         0x83 ), 
    new STReg( stv0910_regs.RSTV0910_P1_TSDIVN,         0x03 ), 		// default
    new STReg( stv0910_regs.RSTV0910_P2_TSDIVN,         0x03 ), 

    new STReg( stv0910_regs.RSTV0910_P1_TSSTATUS2,      0x02 ), 		// *
    new STReg( stv0910_regs.RSTV0910_P2_TSSTATUS2,      0x02 ), 		// *




///
///    new STReg( stv0910_regs.RSTV0910_TSTRES0,           0x00 ), /* TSTRES0          */
///    new STReg( stv0910_regs.RSTV0910_TSTOUT,            0x00 ), /* TSTOUT           */
///    new STReg( stv0910_regs.RSTV0910_TSTIN,             0x00 ), /* TSTIN            */

// SR scan range

///    new STReg( stv0910_regs.RSTV0910_P2_SFRUP1,         0x3f ), /* P2_SFRUP1 */
///    new STReg( stv0910_regs.RSTV0910_P2_SFRUP0,         0xff ), /* P2_SFRUP0 */
///    new STReg( stv0910_regs.RSTV0910_P1_SFRLOW1,        0x2e ), /* P2_SFRLOW1 */
///    new STReg( stv0910_regs.RSTV0910_P2_SFRLOW1,        0x2e ), /* P2_SFRLOW1 */

/*
    new STReg( stv0910_regs.RSTV0910_P1_SFRUP1,         0x80 ), 
    new STReg( stv0910_regs.RSTV0910_P1_SFRUP0,         0x00 ), 
    new STReg( stv0910_regs.RSTV0910_P1_SFRLOW1,        0x80 ), 
    new STReg( stv0910_regs.RSTV0910_P1_SFRLOW0,        0x00 ),
    new STReg( stv0910_regs.RSTV0910_P2_SFRUP1,         0x80 ),
    new STReg( stv0910_regs.RSTV0910_P2_SFRUP0,         0x00 ),
    new STReg( stv0910_regs.RSTV0910_P2_SFRLOW1,        0x80 ),
    new STReg( stv0910_regs.RSTV0910_P2_SFRLOW0,        0x00 ),
*/

// wh52
//
    new STReg( stv0910_regs.RSTV0910_P2_TSTDMD,         0x00 ), /* P2_TSTDMD */
    new STReg( stv0910_regs.RSTV0910_P2_TCTL1,          0x00 ), /* P2_TCTL1 */
    /* TST P2 Registers */
    new STReg( stv0910_regs.RSTV0910_P2_TCTL4,          0x00 ), /* P2_TCTL4 */
    new STReg( stv0910_regs.RSTV0910_P2_TPKTDELIN,      0x00 ), /* P2_TPKTDELIN */
    new STReg( stv0910_regs.RSTV0910_P1_TSTDMD,         0x00 ), /* P1_TSTDMD */
    new STReg( stv0910_regs.RSTV0910_P1_TCTL1,          0x00 ), /* P1_TCTL1 */
    /* TST P1 Registers */
    new STReg( stv0910_regs.RSTV0910_P1_TCTL4,          0x00 ), /* P1_TCTL4 */
    new STReg( stv0910_regs.RSTV0910_P1_TPKTDELIN,      0x00 ), /* P1_TPKTDELIN */

    new STReg( stv0910_regs.RSTV0910_TSTTSRS,           0x00 ), /* TSTTSRS          */        };

    }
}